using System.Text;
using Confluent.Kafka;
using DialogService.Models.Generic.Responses;
using KafkaTestLib.KafkaException;
using KafkaTestLib.KafkaException.ConsumerException;
using Newtonsoft.Json;
using UserService.Models.Account.Requests;
using UserService.Models.Roles.Requests;
using UserService.Services.Account;
using UserService.Services.Roles;
using UserService.Services.UserInfoService;
namespace KafkaTestLib.Kafka;

public class KafkaService
{
    private readonly IConsumer<string, string> _consumer; 
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaService> _logger;
    private readonly KafkaTopicManager _kafkaTopicManager;
    private readonly IAccountService _accountService;
    private readonly IRolesService _rolesService;
    private readonly IUserInfoService _userService;
    private readonly string _userResponseTopic = Environment.GetEnvironmentVariable("ACCOUNTRESP_TOPIC") ?? "accountResponsesTopic";
    
    public KafkaService(ILogger<KafkaService> logger, IProducer<string, string> producer, IConsumer<string, string> consumer, KafkaTopicManager kafkaTopicManager, IAccountService accountService, IRolesService rolesService, IUserInfoService userService)
    {
        _consumer = consumer;
        _producer = producer;
        _logger = logger;
        _kafkaTopicManager = kafkaTopicManager;
        _accountService = accountService;
        _rolesService = rolesService;
        _userService = userService;
        bool isTopicAvailable = IsTopicAvailable(Environment.GetEnvironmentVariable("ACCOUNTREQ_TOPIC") ?? "accountRequestsTopic");
        if(isTopicAvailable)
        {
            _logger.LogInformation("Subscribing to topic");
            _consumer.Subscribe(Environment.GetEnvironmentVariable("ACCOUNTREQ_TOPIC") ?? "accountRequestsTopic");
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
                ConsumeResult<string, string> result = _consumer.Consume();

                if (result != null)
                {
                    // Get the method header bytes
                    var headerBytes = result.Message.Headers
                        .FirstOrDefault(x => x.Key.Equals("method")) ?? throw new NullReferenceException("headerBytes is null");
                    
                    // Convert the bytes to a string
                    var methodString = Encoding.UTF8.GetString(headerBytes.GetValueBytes());
                    
                    Thread.Sleep(5000);
                    switch (methodString)
                    {
                        
                        case "finishRegistration":

                            try
                            {
                                var finishRegistrationRequest = JsonConvert.DeserializeObject<AccountFinishRegistrationRequest>(result.Message.Value)  ?? throw new NullReferenceException("Deserialization failed");
                                if(await _accountService.AccountFinishRegistration(finishRegistrationRequest))
                                {
                                    if(await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Registration successful",}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("finishRegistration")),
                                            new Header("sender", Encoding.UTF8.GetBytes("AccountService"))
                                        ]}))
                                    {
                                        _consumer.Commit(result);
                                    }
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
                                await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error finishing registration",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("finishRegistration")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error finishing registration")),
                                    ]});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                        case "login":

                            try
                            {
                                var loginRequest = JsonConvert.DeserializeObject<AccountLoginRequest>(result.Message.Value)  ?? throw new NullReferenceException("Deserialization failed");
                                if(await _accountService.AccountLogin(loginRequest))
                                {
                                    if(await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Login successful",}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("login")),
                                            new Header("sender", Encoding.UTF8.GetBytes("AccountService"))
                                        ]}))
                                    {
                                        _consumer.Commit(result);
                                    }
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
                                await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error logging in",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("login")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error logging in")),    
                                    ]});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                        case "register":

                            try
                            {
                                var registerRequest = JsonConvert.DeserializeObject<AccountRegisterRequest>(result.Message.Value)  ?? throw new NullReferenceException("Deserialization failed");
                                if(await _accountService.AccountRegister(registerRequest))
                                {
                                    if(await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Registration successful",}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("register")),
                                            new Header("sender", Encoding.UTF8.GetBytes("AccountService"))
                                        ]}))
                                    {
                                        _consumer.Commit(result);
                                    }
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
                                await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error registering",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("register")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error registering")),    
                                    ]});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                        case "changePassword":

                            try
                            {
                                var changePasswordRequest = JsonConvert.DeserializeObject<AccountChangePasswordRequest>(result.Message.Value)  ?? throw new NullReferenceException("Deserialization failed");
                                if(await _accountService.AccountChangePassword(changePasswordRequest))
                                {
                                    if(await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Password changed",}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("changePassword")),
                                            new Header("sender", Encoding.UTF8.GetBytes("AccountService"))
                                        ]}))
                                    {
                                        _consumer.Commit(result);
                                    }
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
                                await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error changing password",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("changePassword")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error changing password")),    
                                    ]});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                        case "createRole":

                            try
                            {
                                var createRoleRequest = JsonConvert.DeserializeObject<RoleCreateRequest>(result.Message.Value)  ?? throw new NullReferenceException("Deserialization failed");
                                if(await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                Value = JsonConvert.SerializeObject(await _rolesService.CreateRole(createRoleRequest)), 
                                Headers = [
                                    new Header("method", Encoding.UTF8.GetBytes("createRole")),
                                    new Header("sender", Encoding.UTF8.GetBytes("AccountService"))
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
                                await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error creating role",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("createRole")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error creating role")),    
                                    ]});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                        case "deleteRole":

                            try
                            {
                                var deleteRoleRequest = JsonConvert.DeserializeObject<RoleDeleteRequest>(result.Message.Value)  ?? throw new NullReferenceException("Deserialization failed");
                                if(await _rolesService.DeleteRole(deleteRoleRequest))
                                {
                                    if(await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Role deleted",}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("deleteRole")),
                                            new Header("sender", Encoding.UTF8.GetBytes("AccountService"))
                                        ]}))
                                    {
                                        _consumer.Commit(result);
                                    }
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
                                await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error deleting role",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("deleteRole")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error deleting role")),    
                                    ]});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                        case "getRoles":

                            try
                            {
                                if(await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(_rolesService.GetRoles()),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("getRoles")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService"))
                                    ]}))
                                {
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
                                await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error getting roles",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("getRoles")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error getting roles")),    
                                    ]});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                        case "updateRole":

                            try
                            {
                                
                                Thread.Sleep(1000);
                                var updateRoleRequest = JsonConvert.DeserializeObject<RoleUpdateRequest>(result.Message.Value)  ?? throw new NullReferenceException("Deserialization failed");
                                if(await Produce(_userResponseTopic,new Message<string, string>(){ 
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(await _rolesService.UpdateRole(updateRoleRequest)),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("updateRole")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService"))
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
                                await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error updating role",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("updateRole")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error updating role")),    
                                    ]});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                        case "getUserByUserName":

                            try
                            {
                                Thread.Sleep(5000);
                                var user = await _userService.GetUserByUsername(result.Message.Value);
                                if(await Produce(_userResponseTopic,new Message<string, string>(){ 
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(user),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("getUserByUserName")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService"))
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
                                await Produce(_userResponseTopic,new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error getting user",}),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("getUserByUserName")),
                                        new Header("sender", Encoding.UTF8.GetBytes("AccountService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error getting user")),    
                                    ]});
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
    public void Dispose()
    {
        _consumer.Dispose();
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