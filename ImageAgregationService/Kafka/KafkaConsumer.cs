using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using ImageAgregationService.Models.RequestModels;
using ImageAgregationService.Services;
using ImageAgregationService.Services.ImageAgregationService;
using KafkaTestLib.KafkaException;
using KafkaTestLib.KafkaException.ConsumerException;
using KafkaTestLib.Models;
using Newtonsoft.Json;
namespace KafkaTestLib.Kafka;

public class KafkaConsumer
{
    private string _topicName;
    private readonly IConsumer<string, string> _consumer; 
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly Offset _offset;
    private readonly KafkaTopicManager _kafkaTopicManager;
    private readonly IImageAgregationService _imageAgregationService;
    
    public KafkaConsumer(ILogger<KafkaConsumer> logger, IConsumer<string, string> consumer, KafkaTopicManager kafkaTopicManager, IImageAgregationService imageAgregationService)
    {
        _topicName = "testTopic";
        _consumer = consumer;
        _logger = logger;
        _kafkaTopicManager = kafkaTopicManager;
        _imageAgregationService = imageAgregationService;
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
                    switch (result.Message.Key)
                    {
                        case string key when key.Contains("generateImage"):
                            try
                            {
                                var message = JsonConvert.DeserializeObject<GenerateImageKafkaRequest>(result.Message.Value);
                                await _imageAgregationService.GetImage(result.Message.Key,message);
                                
                                _consumer.Commit(result);
                                
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e,"Error deserializing message");
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Error deserializing message",e);
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

    
}