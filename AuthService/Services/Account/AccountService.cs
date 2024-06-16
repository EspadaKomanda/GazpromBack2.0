using AuthService.Services.Jwt;
using AuthService.Models.Account.Requests;
using AuthService.Models.Account.Responses;
using AuthService.Services.UserService;
using AuthService.Exceptions.User;
using AuthService.Database.Models;
using Confluent.Kafka;
using BackGazprom.Kafka;

namespace AuthService.Services.Account;

public class AccountService(IJwtService jwtService, IUserService userService, ILogger<AccountService> logger, KafkaRequestResponseService kafkaRequestResponseService) : IAccountService
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
            _logger.LogWarning("Invalid login credentials for user {Username}", request.Username);
            throw new UserInvalidPasswordException();
        }

        var response = new AccountTokensResponse
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user)
        };

        _logger.LogInformation("User {Username} logged in successfully", request.Username);
        return response;
    }

    public async Task<AccountTokensResponse> AccountRefreshToken(AccountRefreshTokenRequest request)
    {
        var validationInfo = await _jwtService.ValidateRefreshToken(request.RefreshToken);

        if (!validationInfo.Item1)
        {
            _logger.LogWarning("Invalid refresh token for user {Username}", validationInfo.Item2);
            throw new InvalidTokenException("Invalid refresh token");
        }

        var user =  await _userService.GetUserByUsername(validationInfo.Item2) ?? throw new UserDoesNotExistException();

        AccountTokensResponse response = new()
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(user)
        };

        _logger.LogInformation("Refreshed token for user {Username}", validationInfo.Item2);
        return response;
    }
   
}
