using System.Text;
using ApiGatewayService.Exceptions.User;
using AuthService.Models.Account.Requests;
using BackGazprom.Models.Generic.Responses;
using Confluent.Kafka;
using KafkaTestLib.Kafka;
using KafkaTestLib.KafkaException;
using Newtonsoft.Json;
using UserService.Services.Account;

namespace ApiGatewayService.Services.Accont
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly KafkaService _kafkaService;
        
        public AccountService(ILogger<AccountService> logger, KafkaService kafkaService)
        {
            _logger = logger;
            _kafkaService = kafkaService;
        }
        public async Task<bool> AccountChangePassword(AccountChangePasswordRequest request)
        {
            try
            {
                Guid messageId = Guid.NewGuid();
                if(await _kafkaService.Produce("accountRequestsTopic",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key = messageId.ToString(),
                    Value = JsonConvert.SerializeObject(request),
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("changePassword")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var changePasswordResponse = await _kafkaService.Consume<MessageResponse>("accountResponsesTopic", messageId, "changePassword");
                    _logger.LogInformation("Change password successful, User: {User}", JsonConvert.SerializeObject(request));
                    return !changePasswordResponse.Message.Contains("Error");
                }
                _logger.LogError("Error sending change password, User: {User}", JsonConvert.SerializeObject(request));
                throw new ChangePasswordException("Error sending change password, User: "+JsonConvert.SerializeObject(request));
            }
            catch (Exception e)
            {
                if (e is not MyKafkaException)
                {
                    _logger.LogError(e,"Error changing password, User: {User}", JsonConvert.SerializeObject(request));
                    throw new ChangePasswordException("Error changing password, User: "+JsonConvert.SerializeObject(request),e);
                }
                _logger.LogError(e,"Unhandled error");
                throw;
            }
        }

        public async Task<bool> AccountFinishRegistration(AccountFinishRegistrationRequest request)
        {
            try
            {
                Guid messageId = Guid.NewGuid();
                if(await _kafkaService.Produce("accountRequestsTopic",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key = messageId.ToString(),
                    Value = JsonConvert.SerializeObject(request),
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("finishRegistration")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var finishRegistrationResponse = await _kafkaService.Consume<MessageResponse>("accountResponsesTopic", messageId, "finishRegistration");
                    _logger.LogInformation("Finish registration successful, User: {User}", JsonConvert.SerializeObject(request));
                    return !finishRegistrationResponse.Message.Contains("Error");
                }
                _logger.LogError("Error sending finish registration, User: {User}", JsonConvert.SerializeObject(request));
                throw new FinishRegistrationException("Error sending finish registration, User: "+JsonConvert.SerializeObject(request));
            }
            catch (Exception e)
            {
                if (e is not MyKafkaException)
                {
                    _logger.LogError(e,"Error finishing registration, User: {User}", JsonConvert.SerializeObject(request));
                    throw new FinishRegistrationException("Error finishing registration, User: "+JsonConvert.SerializeObject(request),e);
                }
                _logger.LogError(e,"Unhandled error");
                throw;
            }
        }

        public async Task<bool> AccountRegister(AccountRegisterRequest request)
        {
            try
            {
                Guid messageId = Guid.NewGuid();
                if(await _kafkaService.Produce("accountRequestsTopic",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key = messageId.ToString(),
                    Value = JsonConvert.SerializeObject(request),
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("register")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var registerResponse = await _kafkaService.Consume<MessageResponse>("accountResponsesTopic", messageId, "register");
                    _logger.LogInformation("Register successful, User: {User}", JsonConvert.SerializeObject(request));
                    return !registerResponse.Message.Contains("Error");
                }
                _logger.LogError("Error sending register, User: {User}", JsonConvert.SerializeObject(request));
                throw new RegisterException("Error sending register, User: "+JsonConvert.SerializeObject(request));
            }
            catch (Exception e)
            {
                if (e is not MyKafkaException)
                {
                    _logger.LogError(e,"Error registering, User: {User}", JsonConvert.SerializeObject(request));
                    throw new RegisterException("Error registering, User: "+JsonConvert.SerializeObject(request),e);
                }
                _logger.LogError(e,"Unhandled error");
                throw;
            }
        }
    }
}