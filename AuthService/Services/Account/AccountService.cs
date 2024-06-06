using AuthService.Services.Jwt;
using BackGazprom.Database.Models;
using BackGazprom.Models.Account.Requests;
using BackGazprom.Models.Account.Responses;
using BackGazprom.Repositories;
using BackGazprom.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BackGazprom.Services.Account;

public class AccountService(IUserRepository userRepo, IUserProfileRepository userProfileRepo, IRegistrationCodeRepository regCodeRepo, IJwtService jwtService) : IAccountService
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IUserProfileRepository _userProfileRepo = userProfileRepo;
    private readonly IRegistrationCodeRepository _regCodeRepo = regCodeRepo;
    private readonly IJwtService _jwtService = jwtService;

    public Task<IActionResult> AccountChangePassword(AccountChangePasswordRequest request)
    {
        throw new NotImplementedException();
    }

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
        return new OkObjectResult(response);
    }

    public Task<ActionResult<AccountTokensResponse>> AccountLogin(AccountLoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult<AccountTokensResponse>> AccountRefreshToken(AccountRefreshTokenRequest request)
    {
        throw new NotImplementedException();
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
    [Obsolete("Method does not implement sending emails yet! SMTP must be integrated")]
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
        return new OkResult();
    }

    Task<ActionResult<AccountTokensResponse>> IAccountService.AccountChangePassword(AccountChangePasswordRequest request)
    {
        throw new NotImplementedException();
    }
}
