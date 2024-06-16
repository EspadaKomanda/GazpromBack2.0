using AuthService.Models.Account.Requests;
using AuthService.Models.Account.Responses;

namespace AuthService.Services.Account;

public interface IAuthService
{
    public Task<AccountTokensResponse> AccountLogin(AccountLoginRequest request);
    public Task<AccountTokensResponse> AccountRefreshToken(AccountRefreshTokenRequest request);
    public Task<Tuple<bool, string>> ValidateRefreshToken(AccountRefreshTokenRequest request);
}