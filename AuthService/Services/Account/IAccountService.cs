using AuthService.Models.Account.Requests;
using AuthService.Models.Account.Responses;

namespace AuthService.Services.Account;

public interface IAccountService
{
    public Task<AccountTokensResponse> AccountLogin(AccountLoginRequest request);
    public Task<AccountTokensResponse> AccountRefreshToken(AccountRefreshTokenRequest request);
}