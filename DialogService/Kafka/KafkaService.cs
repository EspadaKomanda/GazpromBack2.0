using System.Text;
using Confluent.Kafka;
using DialogService.Models.Generic.Responses;
using DialogService.Models.Messages.Requests;
using DialogService.Models.Requests;
using DialogService.Services.DialogsService;
using DialogService.Services.MessagesService;
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
    private readonly IDialogsService _dialogsService;
    private readonly IMessagesService _messageService;
    
    public KafkaService(ILogger<KafkaService> logger, IProducer<string, string> producer, IConsumer<string, string> consumer, KafkaTopicManager kafkaTopicManager, IDialogsService dialogsService, IMessagesService messageService)
    {
        _consumer = consumer;
        _producer = producer;
        _logger = logger;
        _kafkaTopicManager = kafkaTopicManager;
        _dialogsService = dialogsService;
        _messageService = messageService;
        bool isTopicAvailable = IsTopicAvailable("accountRequestsTopic");
        if(isTopicAvailable)
        {
            _consumer.Subscribe("accountRequestsTopic");
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
                        case "clearDialog":
                            try
                            {
                                var request = JsonConvert.DeserializeObject<ClearDialogRequest>(result.Message.Value) ?? throw new NullReferenceException("headerBytes is null");
                                if(await _dialogsService.ClearDialog(request))
                                {
                                    if(await Produce("dialogResponsesTopic", new Message<string, string>(){
                                        Key = result.Message.Key,
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Dialog cleared" }),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("clearDialog")),
                                            new Header("sender", Encoding.UTF8.GetBytes("dialogService"))
                                        ]
                                    }))
                                    {
                                        _logger.LogInformation("Dialog cleared");
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
                                _ = await Produce("dialogResponsesTopic", new Message<string, string>()
                                {
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error clearing dialog" }),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("clearDialog")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error clearing dialog"))
                                    ]
                                });
                                _logger.LogError(e,"Unhandled error");
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            
                            }
                            break;
                        case "createDialog":
                            try
                            {
                                var request = JsonConvert.DeserializeObject<CreateDialogRequest>(result.Message.Value) ?? throw new NullReferenceException("headerBytes is null");
                                if(await Produce("dialogResponsesTopic", new Message<string, string>(){
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(await _dialogsService.CreateDialog(request)),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("createDialog")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService"))
                                    ]
                                }))
                                {
                                    _logger.LogInformation("Dialog created");
                                    _consumer.Commit(result);
                                }
                                
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw;
                                }
                                _ = await Produce("dialogResponsesTopic", new Message<string, string>()
                                {
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error creating dialog" }),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("createDialog")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error creating dialog"))
                                    ]
                                });
                                _logger.LogError(e,"Error creating dialog");
                                _consumer.Commit(result);
                            }    
                            break;
                        case "deleteDialog":
                            try
                            {
                                var request = JsonConvert.DeserializeObject<DeleteDialogRequest>(result.Message.Value) ?? throw new NullReferenceException("headerBytes is null");
                                if(await _dialogsService.DeleteDialog(request)) 
                                {
                                    if(await Produce("dialogResponsesTopic", new Message<string, string>(){
                                        Key = result.Message.Key,
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Dialog deleted" }),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("deleteDialog")),
                                            new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                        ]
                                    }))
                                    {
                                        _logger.LogInformation("Dialog deleted");
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
                                _ = await Produce("dialogResponsesTopic", new Message<string, string>()
                                {
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error deleting dialog" }),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("deleteDialog")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error deleting dialog"))
                                    ]
                                });
                                _logger.LogError(e,"Error deleting dialog");
                                _consumer.Commit(result);
                            }
                            break;
                        case "getDialogsByOwnerId":
                            try
                            {
                                var request = JsonConvert.DeserializeObject<GetDialogsByIdRequest>(result.Message.Value) ?? throw new NullReferenceException("headerBytes is null");
                                if(await Produce("dialogResponsesTopic", new Message<string, string>(){
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(await _dialogsService.GetDialogsByOwnerId(request)),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("getDialogsByOwnerId")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                    ]
                                }))
                                {
                                    _logger.LogInformation("Dialogs found");
                                    _consumer.Commit(result);
                                }
                            
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw;
                                }
                                _ = await Produce("dialogResponsesTopic", new Message<string, string>()
                                {
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error getting dialogs" }),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("getDialogsByOwnerId")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error getting dialogs"))
                                    ]
                                });
                                _logger.LogError(e,"Error getting dialogs");
                                _consumer.Commit(result);
                            }   
                            break;
                        case "getDialogMessages":
                            try
                            {
                                var request = JsonConvert.DeserializeObject<GetDialogMessagesRequest>(result.Message.Value) ?? throw new NullReferenceException("headerBytes is null");
                                if(await Produce("dialogResponsesTopic", new Message<string, string>(){
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(await _dialogsService.GetDialogMessages(request)),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("getDialogMessages")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                    ]
                                }))
                                {
                                    _logger.LogInformation("Messages found");
                                    _consumer.Commit(result);
                                }
                            }  
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw;
                                }
                                _ = await Produce("dialogResponsesTopic", new Message<string, string>()
                                {
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error getting messages" }),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("getDialogMessages")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error getting messages"))
                                    ]
                                });
                                _logger.LogError(e,"Error getting messages");
                                _consumer.Commit(result);
                            }
                            break;
                        case "deleteMessage":
                            try
                            {
                                var request = JsonConvert.DeserializeObject<DeleteMessageRequest>(result.Message.Value) ?? throw new NullReferenceException("headerBytes is null");
                                if(await _messageService.DeleteMessage(request)) 
                                {
                                    if(await Produce("dialogResponsesTopic", new Message<string, string>(){
                                        Key = result.Message.Key,
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Message deleted" }),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("deleteMessage")),
                                            new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                        ]
                                    }))
                                    {
                                        _logger.LogInformation("Message deleted");
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
                                _ = await Produce("dialogResponsesTopic", new Message<string, string>()
                                {
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error deleting message" }),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("deleteMessage")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error deleting message"))
                                    ]
                                });
                                _logger.LogError(e,"Error deleting message");
                                _consumer.Commit(result);
                            }
                            break;
                        case "getMessage":
                            try
                            {
                                var request = JsonConvert.DeserializeObject<GetMessageRequest>(result.Message.Value) ?? throw new NullReferenceException("headerBytes is null");
                                if(await Produce("dialogResponsesTopic", new Message<string, string>(){
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(await _messageService.GetMessage(request)),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("getMessage")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                    ]
                                }))
                                {
                                    _logger.LogInformation("Message found");
                                    _consumer.Commit(result);
                                }
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw;
                                }
                                _ = await Produce("dialogResponsesTopic", new Message<string, string>()
                                {
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error getting message" }),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("getMessage")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error getting message"))
                                    ]
                                });
                                _logger.LogError(e,"Error getting message");
                                _consumer.Commit(result);
                            }
                            break;
                        case "sendMessage":
                            try
                            {
                                var request = JsonConvert.DeserializeObject<SendMessageRequest>(result.Message.Value) ?? throw new NullReferenceException("headerBytes is null");
                                if(await Produce("dialogResponsesTopic", new Message<string, string>(){
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(await _messageService.SendMessage(request)),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("sendMessage")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                    ]
                                }))
                                {
                                    _logger.LogInformation("Message sent");
                                    _consumer.Commit(result);
                                }
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw;
                                }
                                _ = await Produce("dialogResponsesTopic", new Message<string, string>()
                                {
                                    Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = "Error sending message" }),
                                    Headers = [
                                        new Header("method", Encoding.UTF8.GetBytes("sendMessage")),
                                        new Header("sender", Encoding.UTF8.GetBytes("dialogService")),
                                        new Header("error", Encoding.UTF8.GetBytes("Error sending message"))
                                    ]
                                });
                                _logger.LogError(e,"Error sending message");
                                _consumer.Commit(result);
                            }
                            break;
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
                
            }
            else
            {
                _logger.LogError(ex,"Unhandled error");
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