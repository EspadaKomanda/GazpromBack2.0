using System.Text;
using AuthService.Models.Account.Requests;
using AuthService.Services.Account;
using AuthService.Services.Jwt;
using Confluent.Kafka;
using DialogService.Models.Generic.Responses;
using KafkaTestLib.KafkaException;
using KafkaTestLib.KafkaException.ConsumerException;
using Newtonsoft.Json;
namespace KafkaTestLib.Kafka;

public class KafkaService
{
    private readonly IConsumer<string, string> _consumer; 
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaService> _logger;
    private readonly KafkaTopicManager _kafkaTopicManager;
    private readonly IAccountService _accountService;
    private readonly IJwtService _jwtService;
    private readonly string _authResponseTopic = Environment.GetEnvironmentVariable("AUTH_RESPONSE_TOPIC") ?? "authResponseTopic";
    
    public KafkaService(ILogger<KafkaService> logger, IProducer<string, string> producer, IConsumer<string, string> consumer, KafkaTopicManager kafkaTopicManager,AccountService accountService, IJwtService jwtService)
    {
        _consumer = consumer;
        _producer = producer;
        _logger = logger;
        _kafkaTopicManager = kafkaTopicManager;
        _accountService = accountService;
        _jwtService = jwtService;
        bool isTopicAvailable = IsTopicAvailable(Environment.GetEnvironmentVariable("AUTH_REQUESTS_TOPIC") ?? "authRequestsTopic");
        if(isTopicAvailable)
        {
            _consumer.Subscribe(Environment.GetEnvironmentVariable("AUTH_REQUESTS_TOPIC") ?? "authRequestsTopic");
        }
        else
        {
            _logger.LogError("Unable to subscribe to topic");
            throw new ConsumerTopicUnavailableException("Topic unavailable");
        }
    }

    private bool IsTopicAvailable(string topicName)
    {
        try
        {
             bool IsTopicExists = _kafkaTopicManager.CheckTopicExists(topicName);
                if (IsTopicExists)
                {
                    return IsTopicExists;
                }
                _logger.LogError("Unable to subscribe to topic");
                throw new ConsumerTopicUnavailableException("Topic unavailable");
           
        }
        catch (Exception e)
        {
            if (e is MyKafkaException)
            {
                _logger.LogError(e,"Error checking topic");
                throw new ConsumerException("Error checking topic",e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }
    public async Task Consume()
    {
        try
        {
            while (true)
            {
                ConsumeResult<string, string> result = _consumer.Consume(5000);

                if (result != null)
                {
                    // Get the method header bytes
                    var headerBytes = result.Message.Headers
                        .FirstOrDefault(x => x.Key.Equals("method")) ?? throw new NullReferenceException("headerBytes is null");
                    
                    // Convert the bytes to a string
                    var methodString = Encoding.UTF8.GetString(headerBytes.GetValueBytes());
                        
                    switch (methodString)
                    {
                        case "validateRefresh":
                            try
                            {
                                var refreshTokenRequest = JsonConvert.DeserializeObject<AccountRefreshTokenRequest>(result.Message.Value) ?? throw new NullReferenceException("Deserialization failed");
                                var validateResult = await _jwtService.ValidateRefreshToken(refreshTokenRequest.RefreshToken);
                                if(validateResult.Item1)
                                {
                                    if(await Produce(_authResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(validateResult), 
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("validateRefresh")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AuthService"))
                                    ]}))
                                    {
                                        _logger.LogInformation("Message delivery status: Persisted {Result}", result.Message.Value );
                                        _consumer.Commit(result);
                                    }
                                }
                                
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw;
                                }
                                await Produce(_authResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error validating refresh token",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("validateRefresh")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AuthService")),
                                         new Header("error", Encoding.UTF8.GetBytes("Error validating refresh token"))
                                        
                                ]});
                                _logger.LogError(e,"Error validating refresh token");
                                _consumer.Commit(result);
                            }
                            break;
                        case "login":
                            try
                            {
                                var loginRequest = JsonConvert.DeserializeObject<AccountLoginRequest>(result.Message.Value)  ?? throw new NullReferenceException("Deserialization failed");
                                if(await Produce(_authResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                Value = JsonConvert.SerializeObject(await _accountService.AccountLogin(loginRequest)), 
                                Headers = [
                                    new Header("method", Encoding.UTF8.GetBytes("login")),
                                    new Header("sender", Encoding.UTF8.GetBytes("AuthService"))
                                ]}))
                                {
                                    _logger.LogInformation("Message delivery status: Persisted {Result}", result.Message.Value );
                                    _consumer.Commit(result);
                                }
                                break;
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw;
                                }
                                await Produce(_authResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error logging in",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("login")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AuthService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error logging in")),    
                                    ]});
                                _logger.LogError(e,"Error logging in");
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                        case "refreshToken":
                            try
                            {
                                var refreshTokenRequest = JsonConvert.DeserializeObject<AccountRefreshTokenRequest>(result.Message.Value)  ?? throw new NullReferenceException("Deserialization failed");
                                if(await Produce("authResponseTopic",new Message<string, string>(){ Key = result.Message.Key,
                                Value = JsonConvert.SerializeObject(await _accountService.AccountRefreshToken(refreshTokenRequest)),
                                Headers = [
                                    new Header("method", Encoding.UTF8.GetBytes("refreshToken")),
                                    new Header("sender", Encoding.UTF8.GetBytes("AuthService"))
                                ]}))
                                {
                                    _logger.LogInformation("Message delivery status: Persisted {Result}", result.Message.Value );
                                    _consumer.Commit(result);
                                }
                                break;
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw;
                                }
                                await Produce(_authResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error refreshing token",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("refreshToken")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AuthService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error refreshing token")),    
                                    ]});
                                _logger.LogError(e,"Error refreshing token");
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                        default:
                            _consumer.Commit(result);
                            
                            throw new ConsumerRecievedMessageInvalidException("Invalid message received");
                            
                    }

                }
            }
        }
        catch(Exception ex)
        {
            if (ex is MyKafkaException)
            {
                _logger.LogError(ex,"Consumer error");
                throw new ConsumerException("Consumer error ",ex);
            }
            else
            {
                _logger.LogError(ex,"Unhandled error");
                throw;
            }
        }
    }

    
    public async Task<bool> Produce( string topicName,Message<string, string> message)
    {
        try
        {
            bool IsTopicExists = IsTopicAvailable(topicName);
            if (IsTopicExists)
            {
                var deliveryResult = await _producer.ProduceAsync(topicName, message);
                if (deliveryResult.Status == PersistenceStatus.Persisted)
                {
    
                    _logger.LogInformation("Message delivery status: Persisted {Result}", deliveryResult.Value);
                    return true;
                }
                
                _logger.LogError("Message delivery status: Not persisted {Result}", deliveryResult.Value);
                throw new MessageProduceException("Message delivery status: Not persisted" + deliveryResult.Value);
                
            }
            
            bool IsTopicCreated = _kafkaTopicManager.CreateTopic(topicName, Convert.ToInt32(Environment.GetEnvironmentVariable("PARTITIONS_STANDART")), Convert.ToInt16(Environment.GetEnvironmentVariable("REPLICATION_FACTOR_STANDART")));
            if (IsTopicCreated)
            {
                var deliveryResult = await _producer.ProduceAsync(topicName, message);
                if (deliveryResult.Status == PersistenceStatus.Persisted)
                {
                    _logger.LogInformation("Message delivery status: Persisted {Result}", deliveryResult.Value);
                    return true;
                }
                
                _logger.LogError("Message delivery status: Not persisted {Result}", deliveryResult.Value);
                throw new MessageProduceException("Message delivery status: Not persisted");
                
            }
            _logger.LogError("Topic unavailable");
            throw new MessageProduceException("Topic unavailable");
        }
        catch (Exception e)
        {
            if (e is MyKafkaException)
            {
                _logger.LogError(e, "Error producing message");
                throw new ProducerException("Error producing message",e);
            }
            throw;
        }
       
       
        
    }

    
}