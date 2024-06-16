using UserService.Database.Models;
using UserService.Models.Account.Requests;
using UserService.Repositories;
using UserService.Exceptions.AccountExceptions;
using UserService.Utils;
using UserService.Models.Smtp;
using UserService.Services.Roles;

namespace UserService.Services.Account;

public class AccountService(IUserRepository userRepo, IUserProfileRepository userProfileRepo, IRegistrationCodeRepository regCodeRepo, ILogger<AccountService> logger, ISmtpService smtpService, IRoleRepository roleRepository) : IAccountService
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IUserProfileRepository _userProfileRepo = userProfileRepo;
    private readonly IRegistrationCodeRepository _regCodeRepo = regCodeRepo;
    private readonly ILogger<AccountService> _logger = logger;
    private readonly ISmtpService _smtpService = smtpService;
    private readonly IRoleRepository _rolesService = roleRepository;

    public async Task<bool> AccountFinishRegistration(AccountFinishRegistrationRequest request)
    {
        var regcode = await _regCodeRepo.GetRegistrationCodeByCode(request.RegistrationCode);

        if (regcode == null || regcode.ExpirationDate < DateTime.UtcNow)
        {
            // Code does not exist or expired
            throw new InvalidCodeException("Registration code does not exist or expired");
        }

        var existingUser = await _userRepo.GetUserByUsername(request.Username);
        if (existingUser != null)
        {
            // Username already taken
            throw new UsernameExistsException("Username already taken");
        }

        // Creating user
        var user = new User
        {
           Username = request.Username,
           Email = regcode.Email,
           Password = BcryptUtils.HashPassword(request.Password),
           RegistrationDate = DateTime.UtcNow,
           PasswordChangeDate = DateTime.UtcNow,
           Role = _rolesService.GetRoles().First(x=>x.Name == "user")
        };

        try
        {
            await _userRepo.CreateUser(user);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to add user {Username} ({Email}) to database: {Error}" , user.Username, user.Email, e.Message);
            throw;
        }

        // Creating profile
        var UserProfile = new UserProfile
        {
            User = user,
            FirstName = request.FirstName,
            LastName = request.LastName,
            About = request.About
            
        };

        try
        {
            await _userProfileRepo.CreateUserProfile(UserProfile);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to add user profile for user {Username} to database: {Error}", user.Username, e.Message);
            await _userRepo.DeleteUser(user);
            throw;
        }

        try
        {
            await _regCodeRepo.DeleteRegistrationCode(regcode);
        }
        catch (Exception e)
        {
            if (_regCodeRepo.GetRegistrationCodeById(regcode.Id) != null)
            {
                _logger.LogError("Failed to delete registration code from database: {Error}", e.Message);
                await _userRepo.DeleteUser(user);
                throw;
            }

            _logger.LogWarning("Registration code was already deleted: {Error}", e.Message);
        }

        _logger.LogInformation("User {Username} finished registration", user.Username);
        return true;
    }

    public async Task<bool> AccountLogin(AccountLoginRequest request)
    {
        var user = await _userRepo.GetUserByUsername(request.Username);

        if (user == null)
        {
            _logger.LogWarning("Failed login attempt for unexistent user {Username}", request.Username);
            throw new LoginException("Invalid username or password");
        }

        if (!BcryptUtils.VerifyPassword(request.Password, user.Password))
        {
            _logger.LogWarning("Failed login attempt for user {Username}", request.Username);
            throw new LoginException("Invalid username or password");
        }

        return true;
    }

    public async Task<bool> AccountRegister(AccountRegisterRequest request)
    {
        var existingUser = await _userRepo.GetUserByEmail(request.Email);

        //Pretend to register if account already exists
        if (existingUser != null)
        {
            _logger.LogWarning("Attempted registration for {Email} (user already exists)", request.Email);
            return true;
        }

        var existingRegCode = await _regCodeRepo.GetRegistrationCodeByEmail(request.Email);

        if (existingRegCode != null)
        {
            if (existingRegCode.ExpirationDate > DateTime.UtcNow)
            {
                _logger.LogWarning("Attempted registration for {Email} (registration code is not expired)", request.Email);
                //Existing registration code is still valid
                throw new RegistrationLimitException("Too many requests for registration of this email address");
            }
            else
            {
                //Deleting the expired code
                await _regCodeRepo.DeleteRegistrationCode(existingRegCode);
            }
        }

        var newRegCode = new RegistrationCode()
        {
            Code = Guid.NewGuid().ToString(),
            Email = request.Email,
            ExpirationDate = DateTime.UtcNow.AddHours(4)
        };
        try
        {
            await _regCodeRepo.CreateRegistrationCode(newRegCode);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to create registration code for {Email}: {Error}", request.Email, e);
            throw;
        }

        _smtpService.SendSystemMail(new EmailModel() {
            To = request.Email,
            Subject = "Account registration",
            Body = $"Please use code {newRegCode.Code} to register your account"
        });

        _logger.LogInformation("User {Email} requested registration email", request.Email);
        return true;
    }
    
    public async Task<bool> AccountChangePassword(AccountChangePasswordRequest request)
    {
        var user = await _userRepo.GetUserByUsername(request.Username);
        if (user == null)
        {
            throw new TokenException("Could not find the user for the respective token in the database");
        }

        if (request.OldPassword == request.NewPassword)
        {
            throw new PasswordMatchException("The new password must be different from the old password.");
        }

        if (!BcryptUtils.VerifyPassword(request.OldPassword, user.Password))
        {
            throw new InvalidOldPasswordException("The old password is incorrect.");
        }

        user.Password = BcryptUtils.HashPassword(request.NewPassword);
        user.PasswordChangeDate = DateTime.UtcNow;
        await _userRepo.UpdateUser(user);
        
        return true;
    }
}
