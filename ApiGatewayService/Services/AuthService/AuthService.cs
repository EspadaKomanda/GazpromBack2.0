using System.Text;
using ApiGatewayService.Exceptions.User;
using AuthService.Models.Account.Requests;
using AuthService.Models.Account.Responses;
using AuthService.Services.Account;
using Confluent.Kafka;
using KafkaTestLib.Kafka;
using KafkaTestLib.KafkaException;
using Newtonsoft.Json;

namespace AuthService.Services.Auth;
public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly KafkaService _kafkaService;
    public AuthService( ILogger<AuthService> logger, KafkaService kafkaService)
    {
        _logger = logger;
        _kafkaService = kafkaService;
    }
    public async Task<AccountTokensResponse> AccountLogin(AccountLoginRequest request)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce("authRequestsTopic",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(request),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("login")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var loginResponse = await _kafkaService.Consume<AccountTokensResponse>("authResponseTopic", messageId, "login");
                _logger.LogInformation("Login successful, User: {User}", JsonConvert.SerializeObject(request));
                return loginResponse;
            }
            _logger.LogError("Error sending login, User: {User}", JsonConvert.SerializeObject(request));
            throw new LoginException("Error sending login, User: "+JsonConvert.SerializeObject(request));
        } 
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error logging in, User: {User}", JsonConvert.SerializeObject(request));
                throw new LoginException("Error logging in, User: "+JsonConvert.SerializeObject(request),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }

    public async Task<AccountTokensResponse> AccountRefreshToken(AccountRefreshTokenRequest request)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce("authRequestsTopic",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(request),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("refreshToken")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var loginResponse = await _kafkaService.Consume<AccountTokensResponse>("authResponseTopic", messageId, "refreshToken");
                _logger.LogInformation("Refresh token successful, User: {User}", JsonConvert.SerializeObject(request));
                return loginResponse;
            }
            _logger.LogError("Error sending refresh token, User: {User}", JsonConvert.SerializeObject(request));
            throw new RefreshTokenException("Error sending refresh token, User: "+JsonConvert.SerializeObject(request));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error refreshing token, User: {User}", JsonConvert.SerializeObject(request));
                throw new RefreshTokenException("Error refreshing token, User: "+JsonConvert.SerializeObject(request),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }
}