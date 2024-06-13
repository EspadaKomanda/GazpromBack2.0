﻿using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using ImageAgregationService.Models.RequestModels.Mark;
using KafkaTestLib.KafkaException;
using KafkaTestLib.KafkaException.ConsumerException;
using KafkaTestLib.Models;
using Newtonsoft.Json;
namespace KafkaTestLib.Kafka;

public class KafkaService 
{
    private readonly IConsumer<string, string> _consumer; 
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaService> _logger;
    private readonly KafkaTopicManager _kafkaTopicManager;
    
    public KafkaService(ILogger<KafkaService> logger, IProducer<string, string> producer, IConsumer<string, string> consumer, KafkaTopicManager kafkaTopicManager)
    {
        _consumer = consumer;
        _producer = producer;
        _logger = logger;
        
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
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error checking topic");
                throw new ConsumerException("Error checking topic",e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }

    public async Task<T> Consume<T>(string topicName, Guid messageId, string methodName)
    {
        try
        {
            if (!IsTopicAvailable(topicName))
            {
                _logger.LogError("Unable to subscribe to topic");
                throw new ConsumerTopicUnavailableException("Topic unavailable");
            }

            _consumer.Subscribe(topicName);
            while (true)
            {
                ConsumeResult<string, string> result = _consumer.Consume(5000);

                if (result != null)
                {
                    try
                    {
                        if(result.Message.Key == messageId.ToString())
                        {
                            if(Encoding.UTF8.GetString(result.Message.Headers.FirstOrDefault(x => x.Key.Equals("method")).GetValueBytes()) == methodName)
                            {
                                var message = JsonConvert.DeserializeObject<T>(result.Message.Value);
                                _consumer.Commit(result);
                                return message;
                            }
                            _logger.LogError("Wrong message method");
                            throw new ConsumerException("Wrong message method");
                        }   
                    }
                    catch (Exception e)
                    {
                        if (!(e is MyKafkaException))
                        {
                            _logger.LogError(e,"Consumer error");
                            throw new ConsumerException("Consumer error ",e);
                        }
                        _logger.LogError(e,"Unhandled error");
                        throw;
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
            if (e is not MyKafkaException)
            {
                _logger.LogError(e, "Error producing message");
                throw new ProducerException("Error producing message",e);
            }
            throw;
        }
    }

    
}