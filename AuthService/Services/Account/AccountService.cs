using BackGazprom.Database.Models;
using BackGazprom.Models.Account.Requests;
using BackGazprom.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackGazprom.Services.Account;

public class AccountService(IUserRepository userRepo, IRegistrationCodeRepository regCodeRepo) : IAccountService
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IRegistrationCodeRepository _regCodeRepo = regCodeRepo;

    public Task<IActionResult> AccountChangePassword(AccountChangePasswordRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> AccountFinishRegistration(AccountFinishRegistrationRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> AccountLogin(AccountLoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> AccountRefreshToken(AccountRefreshTokenRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IActionResult> AccountRegister(AccountRegisterRequest request)
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
        return new OkResult();
    }
}
