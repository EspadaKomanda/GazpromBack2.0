using BackGazprom.Models.Account.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BackGazprom.Services.Account;

public interface IAccountService
{
    public Task<IActionResult> AccountRegister(AccountRegisterRequest request);
    public Task<IActionResult> AccountFinishRegistration(AccountFinishRegistrationRequest request);
    public Task<IActionResult> AccountLogin(AccountLoginRequest request);
    public Task<IActionResult> AccountChangePassword(AccountChangePasswordRequest request);
    public Task<IActionResult> AccountRefreshToken(AccountRefreshTokenRequest request);
}