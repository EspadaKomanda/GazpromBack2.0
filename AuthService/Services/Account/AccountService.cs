using AuthService.Services.Jwt;
using AuthService.Database.Models;
using AuthService.Models.Account.Requests;
using AuthService.Models.Account.Responses;
using AuthService.Repositories;
using AuthService.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Services.Account;

public class AccountService(IUserRepository userRepo, IUserProfileRepository userProfileRepo, IRegistrationCodeRepository regCodeRepo, IJwtService jwtService, ILogger<AccountService> logger) : IAccountService
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IUserProfileRepository _userProfileRepo = userProfileRepo;
    private readonly IRegistrationCodeRepository _regCodeRepo = regCodeRepo;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger<AccountService> _logger = logger;

    /// <summary>
    /// Finishes the registration of a new account.
    /// </summary>
    /// <param name="request">The request object containing the registration information.</param>
    /// <returns>
    /// An asynchronous task that returns an action result.
    /// The result is either an <see cref="ActionResult{AccountTokensResponse}"/> if the registration is successful,
    /// or an <see cref="UnauthorizedResult"/> if the registration code is invalid or expired,
    /// or a <see cref="StatusCodeResult"/> with a status code of 409 if the username is already taken.
    /// </returns>
    public async Task<ActionResult<AccountTokensResponse>> AccountFinishRegistration(AccountFinishRegistrationRequest request)
    {
        var regcode = await _regCodeRepo.GetRegistrationCodeByCode(request.RegistrationCode);

        if (regcode == null || regcode.ExpirationDate < DateTime.Now)
        {
            // Code does not exist or expired
            return new UnauthorizedResult();
        }

        var existingUser = await _userRepo.GetUserByUsername(request.Username);
        if (existingUser != null)
        {
            // Username already taken
            return new StatusCodeResult(409);
        }

        // Creating user
        var user = new User
        {
           Username = request.Username,
           Email = regcode.Email,
           Password = BcryptUtils.HashPassword(request.Password),
           RegistrationDate = DateTime.Now,
           PasswordChangeDate = DateTime.Now
        };
        await _userRepo.CreateUser(user);

        // Creating profile
        var UserProfile = new UserProfile
        {
            User = user,
            FirstName = request.FirstName,
            LastName = request.LastName,
            About = request.About,
        };
        await _userProfileRepo.CreateUserProfile(UserProfile);

        // Granting tokens
        AccountTokensResponse response = new()
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user)
        };

        _logger.LogInformation("User {Username} finished registration", user.Username);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Logs in a user with the provided username and password.
    /// </summary>
    /// <param name="request">The login request containing the username and password.</param>
    /// <returns>
    /// An asynchronous task that returns an action result.
    /// The result is either an <see cref="OkObjectResult"/> with the account tokens response if the login is successful,
    /// or an <see cref="UnauthorizedObjectResult"/> with an error message if the username or password is invalid.
    /// </returns>
    public async Task<ActionResult<AccountTokensResponse>> AccountLogin(AccountLoginRequest request)
    {
        var user = await _userRepo.GetUserByUsername(request.Username);

        if (user == null)
        {
            return new UnauthorizedObjectResult("Invalid username or password");
        }

        if (!BcryptUtils.VerifyPassword(request.Password, user.Password))
        {
            return new UnauthorizedObjectResult("Invalid username or password");
        }

        AccountTokensResponse response = new()
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user)
        };
        return new OkObjectResult(response);

    }

    /// <summary>
    /// Refreshes the access and refresh tokens for a user.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="request">The request containing the refresh token.</param>
    /// <returns>An asynchronous task that returns an action result with the account tokens response if the refresh is successful,
    /// or a not found result if the user is not found.</returns>
    public async Task<ActionResult<AccountTokensResponse>> AccountRefreshToken(string username, AccountRefreshTokenRequest request)
    {
        var user =  await _userRepo.GetUserByUsername(username);

        // May happen if the user is deleted for some reason
        if (user == null)
        {
            return new NotFoundObjectResult("User not found");
        }

        // Grant the tokens
        AccountTokensResponse response = new()
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user)
        };
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The registration request containing the user's email.</param>
    /// <returns>
    /// An asynchronous task that returns an action result.
    /// The result is either an <see cref="OkResult"/> if the registration is successful,
    /// or a <see cref="StatusCodeResult"/> with a status code of 429 if the existing registration code is still valid,
    /// or a <see cref="OkResult"/> if the registration code is expired.
    /// </returns>
    public async Task<ActionResult> AccountRegister(AccountRegisterRequest request)
    {
        var existingUser = await _userRepo.GetUserByEmail(request.Email);

        //Pretend to register if account already exists
        if (existingUser == null)
        {
            return new OkResult();
        }

        var existingRegCode = await _regCodeRepo.GetRegistrationCodeByEmail(request.Email);

        if (existingRegCode != null)
        {
            if (existingRegCode.ExpirationDate > DateTime.Now)
            {
                //Existing registration code is still valid
                return new StatusCodeResult(429);
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
            ExpirationDate = DateTime.Now.AddHours(4)
        };
        await _regCodeRepo.CreateRegistrationCode(newRegCode);

        //todo: Send email

        _logger.LogInformation("User {Email} requested registration email", request.Email);
        return new OkResult();
    }
    
    /// <summary>
    /// Changes the password for a user.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="request">The request containing the old and new passwords.</param>
    /// <returns>
    /// An asynchronous task that returns an action result.
    /// The result is either an <see cref="UnauthorizedResult"/> if the user does not exist,
    /// a <see cref="BadRequestObjectResult"/> if the new password is the same as the old password or if the old password is incorrect,
    /// or an <see cref="OkObjectResult"/> with the new access token and refresh token if the password is changed successfully.
    /// </returns>
    public async Task<ActionResult<AccountTokensResponse>> AccountChangePassword(string username, AccountChangePasswordRequest request)
    {
        var user = await _userRepo.GetUserByUsername(username);
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        if (request.OldPassword == request.NewPassword)
        {
            return new BadRequestObjectResult("New and old passwords must not match.");
        }

        if (!BcryptUtils.VerifyPassword(request.OldPassword, user.Password))
        {
            return new BadRequestObjectResult("Incorrect old password.");
        }

        user.Password = BcryptUtils.HashPassword(request.NewPassword);
        user.PasswordChangeDate = DateTime.Now;
        await _userRepo.UpdateUser(user);
        return new OkObjectResult(new AccountTokensResponse
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user)
        });
    }
}
