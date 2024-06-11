using System.Runtime.InteropServices;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaTestLib.KafkaException;
using KafkaTestLib.Models;

namespace KafkaTestLib.Kafka;

public class KafkaProducer 
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly IProducer<string,string> _producer;
    private readonly KafkaTopicManager _kafkaTopicManager;
    public KafkaProducer(IProducer<string, string> producer, ILogger<KafkaProducer> logger, KafkaTopicManager kafkaTopicManager)
    {
        _producer = producer;
        _logger = logger;
        _kafkaTopicManager = kafkaTopicManager;
    }

    public async Task<ProduceResponseModel> Produce( string topicName,Message<string, string> message)
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
                        return new ProduceResponseModel() { Success = true , ErrorMessage = "", Message = null};
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
                        return new ProduceResponseModel() { Success = true , ErrorMessage = "", Message = null};
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