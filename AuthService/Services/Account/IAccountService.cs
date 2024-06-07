using AuthService.Models.Account.Requests;
using AuthService.Models.Account.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Services.Account;

public interface IAccountService
{
    public Task<ActionResult> AccountRegister(AccountRegisterRequest request);
    public Task<ActionResult<AccountTokensResponse>> AccountFinishRegistration(AccountFinishRegistrationRequest request);
    public Task<ActionResult<AccountTokensResponse>> AccountLogin(AccountLoginRequest request);
    public Task<ActionResult<AccountTokensResponse>> AccountChangePassword(string username, AccountChangePasswordRequest request);
    public Task<ActionResult<AccountTokensResponse>> AccountRefreshToken(AccountRefreshTokenRequest request);
}