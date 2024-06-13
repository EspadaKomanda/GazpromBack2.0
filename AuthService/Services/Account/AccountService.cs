using AuthService.Services.Jwt;
using AuthService.Models.Account.Requests;
using AuthService.Models.Account.Responses;
using AuthService.Services.UserService;
using AuthService.Exceptions.User;

namespace AuthService.Services.Account;

//TODO: Implement methods that contact the UserService
public class AccountService(IJwtService jwtService, IUserService userService, ILogger<AccountService> logger) : IAccountService
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly IUserService _userService = userService;
    private readonly ILogger<AccountService> _logger = logger;

    public async Task<AccountTokensResponse> AccountLogin(AccountLoginRequest request)
    {

        var user = await _userService.GetUserByUsername(request.Username) ?? throw new UserDoesNotExistException();

        var SuccessLogin = await _userService.Login(request);
        if (!SuccessLogin)
        {
            throw new UserInvalidPasswordException();
        }

        var response = new AccountTokensResponse
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user)
        };

        return response;
    }

    public async Task<AccountTokensResponse> AccountRefreshToken(AccountRefreshTokenRequest request)
    {
        var validationInfo = await _jwtService.ValidateRefreshToken(request.RefreshToken);

        if (!validationInfo.Item1)
        {
            throw new InvalidTokenException("Invalid refresh token");
        }

        var user =  await _userService.GetUserByUsername(validationInfo.Item2) ?? throw new UserDoesNotExistException();

        AccountTokensResponse response = new()
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user)
        };
        return response;
    }
}
