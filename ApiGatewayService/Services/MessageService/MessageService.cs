using System.Text;
using ApiGatewayService.Exceptions.MessageExceptions;
using BackGazprom.Models.Generic.Responses;
using Confluent.Kafka;
using DialogService.Database.Models;
using DialogService.Models.Messages.Requests;
using KafkaTestLib.Kafka;
using KafkaTestLib.KafkaException;
using Newtonsoft.Json;

namespace DialogService.Services.MessagesService;

public class MessageService : IMessagesService
{
    private readonly ILogger<MessageService> _logger;
    private readonly KafkaService _kafkaService;
    public MessageService( ILogger<MessageService> logger, KafkaService kafkaService)
    {
        _logger = logger;
        _kafkaService = kafkaService;
    }
    public async Task<bool> DeleteMessage(DeleteMessageRequest obj)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce("dialogResponsesTopic",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(obj),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("deleteMessage")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var dialog = await _kafkaService.Consume<MessageResponse>("dialogResponsesTopic", messageId, "deleteMessage");
                _logger.LogInformation("Message deleted, Message: {Message}", JsonConvert.SerializeObject(obj));
                return !dialog.Message.Contains("Error");
            }
            _logger.LogError("Error sending message, Message: {Message}", JsonConvert.SerializeObject(obj));
            throw new DeleteMessageException("Error sending message, Message: "+JsonConvert.SerializeObject(obj));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error deleting message, Message: {Message}", JsonConvert.SerializeObject(obj));
                throw new DeleteMessageException("Error deleting message, Message: "+JsonConvert.SerializeObject(obj),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }

    public async Task<Message> GetMessage(GetMessageRequest obj)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce("dialogResponsesTopic",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(obj),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("getDialogMessages")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var dialog = await _kafkaService.Consume<MessageResponse>("dialogResponsesTopic", messageId, "getMessage");
                _logger.LogInformation("Message received, Message: {Message}", JsonConvert.SerializeObject(obj));
                return JsonConvert.DeserializeObject<Message>(dialog.Message)!;
            }
            _logger.LogError("Error sending message, Message: {Message}", JsonConvert.SerializeObject(obj));
            throw new GetMessageException("Error sending message, Message: "+JsonConvert.SerializeObject(obj));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error getting message, Message: {Message}", JsonConvert.SerializeObject(obj));
                throw new GetMessageException("Error getting message, Message: "+JsonConvert.SerializeObject(obj),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }

    public async Task<Message> SendMessage(SendMessageRequest obj)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce("dialogResponsesTopic",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(obj),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("sendMessage")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var message = await _kafkaService.Consume<Message>("dialogResponsesTopic", messageId, "sendMessage");
                _logger.LogInformation("Message sent, Message: {Message}", JsonConvert.SerializeObject(obj));
                return message;
            } 
            _logger.LogError("Error sending message, Message: {Message}", JsonConvert.SerializeObject(obj));
            throw new SendMessageException("Error sending message, Message: "+JsonConvert.SerializeObject(obj));  
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error sending message, Message: {Message}", JsonConvert.SerializeObject(obj));
                throw new SendMessageException("Error sending message, Message: "+JsonConvert.SerializeObject(obj),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }
}
