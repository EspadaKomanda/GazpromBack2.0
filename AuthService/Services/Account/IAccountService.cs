using BackGazprom.Models.Account.Requests;
using BackGazprom.Models.Account.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BackGazprom.Services.Account;

public interface IAccountService
{
    public Task<ActionResult> AccountRegister(AccountRegisterRequest request);
    public Task<ActionResult<AccountTokensResponse>> AccountFinishRegistration(AccountFinishRegistrationRequest request);
    public Task<ActionResult<AccountTokensResponse>> AccountLogin(AccountLoginRequest request);
    public Task<ActionResult<AccountTokensResponse>> AccountChangePassword(AccountChangePasswordRequest request);
    public Task<ActionResult<AccountTokensResponse>> AccountRefreshToken(AccountRefreshTokenRequest request);
}