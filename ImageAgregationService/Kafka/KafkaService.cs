﻿using System.Text;
using Confluent.Kafka;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using ImageAgregationService.Models.RequestModels.Mark;
using ImageAgregationService.Services.ImageAgregationService;
using ImageAgregationService.Services.MarkService;
using ImageAgregationService.Services.TemplateService;
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
    private readonly IImageAgregationService _imageAgregationService;
    private readonly ITemplateService _templateService;
    private readonly IMarkService _markService;
    
    public KafkaService(ILogger<KafkaService> logger, IProducer<string, string> producer, IConsumer<string, string> consumer, KafkaTopicManager kafkaTopicManager, IImageAgregationService imageAgregationService, ITemplateService templateService, IMarkService markService)
    {
        _consumer = consumer;
        _producer = producer;
        _logger = logger;
        _kafkaTopicManager = kafkaTopicManager;
        _imageAgregationService = imageAgregationService;
        _templateService = templateService;
        _markService = markService;
        bool isTopicAvailable = IsTopicAvailable("imageRequestsTopic");
        if(isTopicAvailable)
        {
            _consumer.Subscribe("imageRequestsTopic");
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
            if (e is not MyKafkaException)
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
                                    _logger.LogInformation("Successfully sent message",result.Message.Key);
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
                                await Produce("imageResponsesTopic",new Message<string, string>(){ Key =result.Message.Key, 
                                Value = "Error generating image", 
                                Headers = new Headers(){
                                            new Header("method", Encoding.UTF8.GetBytes("generateImage")),
                                            new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService")),
                                            new Header("error", Encoding.UTF8.GetBytes("Error generating image"))
                                }});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                            break;
                        case "getTemplates":
                            try
                            {
                                List<TemplateDto> templates = await _templateService.GetTemplates(JsonConvert.DeserializeObject<GetTemplateKafkaRequest>(result.Message.Value));
                                if(await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key,
                                        Value = JsonConvert.SerializeObject(templates),
                                        Headers = new Headers(){
                                                new Header("method", Encoding.UTF8.GetBytes("getTemplates")),
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
                                await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = "Error getting templates",
                                        Headers = new Headers(){
                                                    new Header("method", Encoding.UTF8.GetBytes("getTemplates")),
                                                    new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService")),
                                                    new Header("error", Encoding.UTF8.GetBytes("Error getting templates"))
                                        }});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                            break;
                        case "addTemplate":
                            try
                            {
                                TemplateDto template = JsonConvert.DeserializeObject<TemplateDto>(result.Message.Value);
                                if(await _templateService.AddTemplate(template))
                                {
                                    if(await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key, 
                                    Value = "Template added",
                                    Headers = new Headers(){
                                                new Header("method", Encoding.UTF8.GetBytes("addTemplate")),
                                                new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService"))
                                    }}))
                                    {
                                        _consumer.Commit(result);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw e;
                                }
                                await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = "Error adding template",
                                        Headers = new Headers(){
                                                    new Header("method", Encoding.UTF8.GetBytes("addTemplate")),
                                                    new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService")),
                                                    new Header("error", Encoding.UTF8.GetBytes("Error adding template"))
                                        }});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                            break;
                        case "deleteTemplate":
                            try
                            {
                                DeleteTemplateKafkaRequest template = JsonConvert.DeserializeObject<DeleteTemplateKafkaRequest>(result.Message.Value);
                                if(await _templateService.DeleteTemplate(template))
                                {
                                    if(await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key,
                                    Value = "Template deleted",
                                    Headers = new Headers(){
                                                new Header("method", Encoding.UTF8.GetBytes("deleteTemplate")),
                                                new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService"))}}))
                                    {

                                    }
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
                                await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = "Error deleting template",
                                        Headers = new Headers(){
                                                    new Header("method", Encoding.UTF8.GetBytes("addTemplate")),
                                                    new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService")),
                                                    new Header("error", Encoding.UTF8.GetBytes("Error deleting template"))
                                        }});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                            break;
                        case "updateTemplate":
                            try
                            {
                                UpdateTemplateKafkaRequest template = JsonConvert.DeserializeObject<UpdateTemplateKafkaRequest>(result.Message.Value);
                                if(await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key,
                                 Value = JsonConvert.SerializeObject(await _templateService.UpdateTemplate(template)),
                                 Headers = new Headers(){
                                            new Header("method", Encoding.UTF8.GetBytes("updateTemplate")),
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
                                await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = "Error deleting template",
                                        Headers = new Headers(){
                                                    new Header("method", Encoding.UTF8.GetBytes("addTemplate")),
                                                    new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService")),
                                                    new Header("error", Encoding.UTF8.GetBytes("Error deleting template"))
                                        }});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                            break;
                        case "updateMark":
                            try
                            {
                                UpdateMarkKafkaRequest updateMark = JsonConvert.DeserializeObject<UpdateMarkKafkaRequest>(result.Message.Value);
                                if(await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key,
                                Value = JsonConvert.SerializeObject(await _markService.UpdateMark(updateMark)),
                                Headers = new Headers(){
                                            new Header("method", Encoding.UTF8.GetBytes("updateMark")),
                                            new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService"))}}))
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
                                await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = "Error updating mark",
                                        Headers = new Headers(){
                                                    new Header("method", Encoding.UTF8.GetBytes("updateMark")),
                                                    new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService")),
                                                    new Header("error", Encoding.UTF8.GetBytes("Error updating mark"))
                                        }});
                                _consumer.Commit(result);
                                throw new ConsumerRecievedMessageInvalidException("Unhandled error",e);
                            }
                            break;
                        case "getImages":
                            try
                            {
                                GetImagesKafkaRequest getImages = JsonConvert.DeserializeObject<GetImagesKafkaRequest>(result.Message.Value);
                                List<ImageDto> images = await _imageAgregationService.GetImages(getImages);
                                if(images!=null)
                                {
                                    if(await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key,
                                    Value = JsonConvert.SerializeObject(images),
                                    Headers = new Headers(){
                                                new Header("method", Encoding.UTF8.GetBytes("getImages")),
                                                new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService"))}}))
                                    {
                                        _consumer.Commit(result);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                if(e is MyKafkaException)
                                {
                                    _logger.LogError(e,"Error sending message");
                                    throw e;
                                }
                                await Produce("imageResponsesTopic",new Message<string, string>(){ Key = result.Message.Key, 
                                        Value = "Error getting images",
                                        Headers = new Headers(){
                                                    new Header("method", Encoding.UTF8.GetBytes("getImages")),
                                                    new Header("sender", Encoding.UTF8.GetBytes("imageAgregationService")),
                                                    new Header("error", Encoding.UTF8.GetBytes("Error getting images"))     
                                }});
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
    
                    _logger.LogInformation("Message delivery status: Persisted " + deliveryResult.Value);
                    return true;
                }
                
                _logger.LogError("Message delivery status: Not persisted " + deliveryResult.Value);
                throw new MessageProduceException("Message delivery status: Not persisted" + deliveryResult.Value);
                
            }
            
            bool IsTopicCreated = _kafkaTopicManager.CreateTopic(topicName, Convert.ToInt32(Environment.GetEnvironmentVariable("PARTITIONS_STANDART")), Convert.ToInt16(Environment.GetEnvironmentVariable("REPLICATION_FACTOR_STANDART")));
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