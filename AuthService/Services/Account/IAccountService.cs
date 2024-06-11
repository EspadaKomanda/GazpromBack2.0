using AuthService.Models.Account.Requests;
using AuthService.Models.Account.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Services.Account;

public interface IAccountService
{
    public Task<bool> AccountRegister(AccountRegisterRequest request);
    public Task<AccountTokensResponse> AccountFinishRegistration(AccountFinishRegistrationRequest request);
    public Task<AccountTokensResponse> AccountLogin(AccountLoginRequest request);
    public Task<AccountTokensResponse> AccountChangePassword(string username, AccountChangePasswordRequest request);
    public Task<AccountTokensResponse> AccountRefreshToken(string username, AccountRefreshTokenRequest request);
}