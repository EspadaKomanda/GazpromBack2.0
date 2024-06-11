using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using ImageAgregationService.Services;
using ImageAgregationService.Services.ImageAgregationService;
using ImageAgregationService.Services.TemplateService;
using KafkaTestLib.KafkaException;
using KafkaTestLib.KafkaException.ConsumerException;
using KafkaTestLib.Models;
using Newtonsoft.Json;
namespace KafkaTestLib.Kafka;

public class KafkaService
{
    private string _topicName;
    private readonly IConsumer<string, string> _consumer; 
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaService> _logger;
    private readonly KafkaTopicManager _kafkaTopicManager;
    private readonly IImageAgregationService _imageAgregationService;
    private readonly ITemplateService _templateService;
    
    public KafkaService(ILogger<KafkaService> logger, IProducer<string, string> producer, IConsumer<string, string> consumer, KafkaTopicManager kafkaTopicManager, IImageAgregationService imageAgregationService, ITemplateService templateService)
    {
        _topicName = "imageRequestsTopic";
        _consumer = consumer;
        _producer = producer;
        _logger = logger;
        _kafkaTopicManager = kafkaTopicManager;
        _imageAgregationService = imageAgregationService;
        _templateService = templateService;
        bool isTopicAvailable = IsTopicAvailable().Result;
        if(isTopicAvailable)
        {
            _consumer.Subscribe(_topicName);
        }
        else
        {
            _logger.LogError("Unable to subscribe to topic");
            throw new ConsumerTopicUnavailableException("Topic unavailable");
        }
    }

    private async Task<bool> IsTopicAvailable()
    {
        try
        {
             bool IsTopicExists = await _kafkaTopicManager.CheckTopicExists(_topicName);
                if (IsTopicExists)
                {
                    return IsTopicExists;
                }
                _logger.LogError("Unable to subscribe to topic");
                throw new ConsumerTopicUnavailableException("Topic unavailable");
           
        }
        catch (Exception e)
        {
            if (!(e is MyKafkaException))
            {
                _logger.LogError(e,"Error checking topic");
                throw new ConsumerException("Error checking topic",e);
            }
            _logger.LogError(e,"Unhandled error");
            throw e;
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
                    switch (Encoding.UTF8.GetString(result.Message.Headers.FirstOrDefault(x => x.Key.Equals("method")).GetValueBytes()))
                    {
                        case "generateImage":
                            try
                            {
                                var message = JsonConvert.DeserializeObject<GenerateImageKafkaRequest>(result.Message.Value);
                                ImageDto image = await _imageAgregationService.GetImage(result.Message.Key,message);
                                if(await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key, 
                                                                                                    Value = JsonConvert.SerializeObject(image), 
                                                                                                    Headers = new Headers(){ 
                                                                                                                new Header("method", Encoding.UTF8.GetBytes("generateImage")),
                                                                                                                new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService")) 
                                                                                                    }}))
                                {
                                    _consumer.Commit(result);
                                }
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw e;
                                }
                                await Produce("generateImageResponse",new Message<string, string>(){ Key =result.Message.Key, 
                                                                                                    Value = "Error generating image", 
                                                                                                    Headers = new Headers(){
                                                                                                                new Header("method", Encoding.UTF8.GetBytes("generateImage")),
                                                                                                                new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService")),
                                                                                                                new Header("error", Encoding.UTF8.GetBytes("Error generating image"))
                                                                                                    }});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Error deserializing message",e);
                            }
                            break;
                        case string key when key.Contains("getTemplates"):
                            try
                            {
                                List<TemplateDto> templates = await _templateService.GetTemplates(JsonConvert.DeserializeObject<GetTemplateKafkaRequest>(result.Message.Value));
                                if(await Produce("getTemplatesResponse",new Message<string, string>(){ Key = "getTemplates"+key.Replace("getTemplates",""), Value = JsonConvert.SerializeObject(templates) }))
                                {
                                    _consumer.Commit(result);
                                }
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw e;
                                }
                                await Produce("getTemplatesResponse",new Message<string, string>(){ Key = "getTemplates_error"+key.Replace("getTemplates",""), Value = "Error getting templates"});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Error deserializing message",e);
                            }
                            break;
                        case string key when key.Contains("getImages"):
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
            if (!(ex is MyKafkaException))
            {
                _logger.LogError(ex,"Consumer error");
                throw new ConsumerException("Consumer error ",ex);
            }
            else
            {
                _logger.LogError(ex,"Unhandled error");
                throw ex;
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
            bool IsTopicExists = await _kafkaTopicManager.CheckTopicExists(topicName);
            if (IsTopicExists)
            {
                var deliveryResult = await _producer.ProduceAsync(topicName, message);
                if (deliveryResult.Status == PersistenceStatus.Persisted)
                {
    
                    _logger.LogInformation("Message delivery status: Persisted " + deliveryResult.Value);
                    return true;
                }
                
                _logger.LogError("Message delivery status: Not persisted " + deliveryResult.Value);
                throw new MessageProduceException("Message delivery status: Not persisted" + deliveryResult.Value);
                
            }
            
            bool IsTopicCreated = await _kafkaTopicManager.CreateTopic(topicName, Convert.ToInt32(Environment.GetEnvironmentVariable("PARTITIONS_STANDART")), Convert.ToInt16(Environment.GetEnvironmentVariable("REPLICATION_FACTOR_STANDART")));
            if (IsTopicCreated)
            {
                var deliveryResult = await _producer.ProduceAsync(topicName, message);
                if (deliveryResult.Status == PersistenceStatus.Persisted)
                {
                    _logger.LogInformation("Message delivery status: Persisted "+ deliveryResult.Value);
                    return true;
                }
                
                _logger.LogError("Message delivery status: Not persisted "+ deliveryResult.Value);
                throw new MessageProduceException("Message delivery status: Not persisted");
                
            }
            _logger.LogError("Topic unavailable");
            throw new MessageProduceException("Topic unavailable");
        }
        catch (Exception e)
        {
            if (!(e is MyKafkaException))
            {
                _logger.LogError(e, "Error producing message");
                throw new ProducerException("Error producing message",e);
            }
            throw e;
        }
       
       
        
    }

    
}