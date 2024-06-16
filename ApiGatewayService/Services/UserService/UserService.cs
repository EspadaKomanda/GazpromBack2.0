using System.Text;
using ApiGatewayService.Exceptions.User;
using AuthService.Database.Models;
using Confluent.Kafka;
using KafkaTestLib.Kafka;
using KafkaTestLib.KafkaException;

namespace UserService.Services.UserInfoService;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly KafkaService _kafkaService;
    public UserService( ILogger<UserService> logger, KafkaService kafkaService)
    {
        _logger = logger;
        _kafkaService = kafkaService;
    }
    public async Task<User?> GetUserByUsername(string username)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce(Environment.GetEnvironmentVariable("ACCOUNTREQ_TOPIC") ?? "",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = username,
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("getUserByUsername")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var user = await _kafkaService.Consume<User>(Environment.GetEnvironmentVariable("ACCOUNTRESP_TOPIC") ?? "", messageId, "getUserByUsername");
                _logger.LogInformation("User found, User: {User}", username);
                return user;
            }
            _logger.LogError("Error sending user, User: {User}", username);
            throw new UserNotFoundException("Error sending user, User: "+username);
        }   
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error getting user, User: {User}", username);
                throw new UserNotFoundException("Error getting user, User: "+username,e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }
}
