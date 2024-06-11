using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using KafkaTestLib.KafkaException;
using KafkaTestLib.KafkaException.ConsumerException;
using KafkaTestLib.Models;
using Newtonsoft.Json;
namespace KafkaTestLib.Kafka;

public class KafkaConsumer : IDisposable
{
    private string _topicName;
    private readonly IConsumer<string, string> _consumer; 
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly KafkaTopicManager _kafkaTopicManager;
    public KafkaConsumer(ILogger<KafkaConsumer> logger, IConsumer<string, string> consumer, KafkaTopicManager kafkaTopicManager, string topicName)
    {
        _topicName = topicName;
        _consumer = consumer;
        _logger = logger;
        _kafkaTopicManager = kafkaTopicManager;
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
    public async Task<T> Consume<T>(string requiredMethodName, string requiredId)
    {
        try
        {
            while (true)
            {
                ConsumeResult<string, string> result = _consumer.Consume(5000);

                if (result != null)
                {
                    if (result.Message.Key == requiredMethodName+requiredId)
                    {
                        try
                        {
                           
                            T deserializedObject = JsonConvert.DeserializeObject<T>(result.Message.Value);

                            _consumer.Commit(result);

                            return deserializedObject;
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Error deserializing message");
                            _consumer.Commit(result);
                            throw new ConsumerRecievedMessageInvalidException("Error deserializing message", e);
                        }
                    }
                    else
                    {
                        _consumer.Commit(result);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (!(ex is MyKafkaException))
            {
                _logger.LogError(ex, "Consumer error");
                throw new ConsumerException("Consumer error", ex);
            }
            else
            {
                _logger.LogError(ex, "Unhandled error");
                throw ex;
            }
        }
    }
    public void Dispose()
    {
        _consumer.Dispose();
    }

    
}