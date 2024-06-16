using System.Text;
using AuthService.Database.Models;
using AuthService.Exceptions.User;
using AuthService.Models.Account.Requests;
using AuthService.Services.UserService;
using BackGazprom.Kafka;
using Confluent.Kafka;
using DialogService.Models.Generic.Responses;
using Newtonsoft.Json;

namespace BackGazprom.Services.UserService;

public class UserService : IUserService
{
    
    private readonly KafkaRequestResponseService _kafkaRequestResponseService;
    private readonly ILogger<UserService> _logger;
    public UserService(KafkaRequestResponseService kafkaRequestResponseService, ILogger<UserService> logger)
    {
        _kafkaRequestResponseService = kafkaRequestResponseService;
        _logger = logger;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaRequestResponseService.Produce(Environment.GetEnvironmentVariable("ACCOUNTREQ_TOPIC") ?? "accountRequestsTopic",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = username,
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("getUserByUserName")),
                    new Header("sender",Encoding.UTF8.GetBytes("authService"))
                }
            }))
            {
                var user = await _kafkaRequestResponseService.Consume<User>(Environment.GetEnvironmentVariable("ACCOUNTRESP_TOPIC") ?? "accountRequestsTopic", messageId, "getUserByUsername");
                _logger.LogInformation("User aquired successefully");
                return user;
            }
            _logger.LogError("User not found!");
            throw new UserDoesNotExistException();
        }
        catch(Exception e)
        {
            _logger.LogError(e,"Geting user failed");
            throw;
        }
    }

    public async Task<bool> Login(AccountLoginRequest request)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaRequestResponseService.Produce(Environment.GetEnvironmentVariable("ACCOUNTREQ_TOPIC"),
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(request),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("login")),
                    new Header("sender",Encoding.UTF8.GetBytes("authService"))
                }
            }))
            {
                var result = await _kafkaRequestResponseService.Consume<MessageResponse>(Environment.GetEnvironmentVariable("ACCOUNTRESP_TOPIC"), messageId, "login");
                _logger.LogInformation(result.Message);
                return !result.Message.Contains("Error");
            }
            _logger.LogError("User not found!");
            throw new UserDoesNotExistException();
        }
        catch(Exception e)
        {
            _logger.LogError(e,"Geting user failed");
            throw;
        }
    }
}
