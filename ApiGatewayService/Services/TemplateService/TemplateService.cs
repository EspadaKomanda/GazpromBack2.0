using System.Text;
using BackGazprom.Models.Generic.Responses;
using Confluent.Kafka;
using ImageAgregationService.Exceptions.TemplateExceptions;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using KafkaTestLib.Kafka;
using KafkaTestLib.KafkaException;
using Newtonsoft.Json;

namespace ImageAgregationService.Services.TemplateService
{
    public class TemplateService : ITemplateService
    {
        private readonly ILogger<TemplateService> _logger;
        private readonly KafkaService _kafkaService;
        public TemplateService(ILogger<TemplateService> logger, KafkaService kafkaService)
        {
            _logger = logger;
            _kafkaService = kafkaService;
        }
        public async Task<bool> AddTemplate(TemplateDto templateDto)
        {
            try
            {
                Guid messageId = Guid.NewGuid();
                if(await _kafkaService.Produce("imageRequestsTopic",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key = messageId.ToString(),
                    Value = JsonConvert.SerializeObject(templateDto),
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("addTemplate")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var addingResult = await _kafkaService.Consume<MessageResponse>("imageResponsesTopic", messageId, "addTemplate");
                    if(addingResult.Message == "Successfully added template")
                    {
                        _logger.LogInformation("Template added successfully, Template: {Template}", JsonConvert.SerializeObject(templateDto));
                        return true;
                    }
                    _logger.LogError("Error adding template, Template: {Template}, Message: {Message}", JsonConvert.SerializeObject(templateDto), addingResult.Message);
                    throw new AddTemplateException("Error adding template, Template: "+JsonConvert.SerializeObject(templateDto) + ", Message: "+addingResult.Message);
                    
                }
                _logger.LogError("Error sending template, Template: {Template}", JsonConvert.SerializeObject(templateDto));
                throw new AddTemplateException("Error sending template, Template: "+JsonConvert.SerializeObject(templateDto));
            }
            catch (Exception e)
            {
                if (e is not MyKafkaException)
                {
                    _logger.LogError(e,"Error adding template, Template: {Template}", JsonConvert.SerializeObject(templateDto));
                    throw new AddTemplateException("Error adding template, Template: "+JsonConvert.SerializeObject(templateDto),e);
                }
                _logger.LogError(e,"Unhandled error");
                throw;
            }
        }

        public async Task<bool> DeleteTemplate(DeleteTemplateKafkaRequest deleteTemplateRequest)
        {
            try
            {
                Guid messageId = Guid.NewGuid();
                if(await _kafkaService.Produce("imageRequestsTopic",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key = messageId.ToString(),
                    Value = JsonConvert.SerializeObject(deleteTemplateRequest),
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("deleteTemplate")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var deletingResult = await _kafkaService.Consume<MessageResponse>("imageResponsesTopic", messageId, "deleteTemplate");
                    if(deletingResult.Message == "Successfully deleted template")
                    {
                        _logger.LogInformation("Template deleted successfully, Template: {Template}", JsonConvert.SerializeObject(deleteTemplateRequest));
                        return true;
                    }
                    _logger.LogError("Error deleting template, Template: {Template}, Message: {Message}", JsonConvert.SerializeObject(deleteTemplateRequest), deletingResult.Message);
                    throw new DeleteTemplateException("Error deleting template, Template: "+JsonConvert.SerializeObject(deleteTemplateRequest) + ", Message: "+deletingResult.Message);
                }
                _logger.LogError("Error sending template, Template: {Template}", JsonConvert.SerializeObject(deleteTemplateRequest));
                throw new DeleteTemplateException("Error sending template, Template: "+JsonConvert.SerializeObject(deleteTemplateRequest));
            }
            catch (Exception e)
            {
                if (e is not MyKafkaException)
                {
                    _logger.LogError(e,"Error deleting template, Template: {Template}", JsonConvert.SerializeObject(deleteTemplateRequest));
                    throw new DeleteTemplateException("Error deleting Template, Template: "+JsonConvert.SerializeObject(deleteTemplateRequest),e);
                }
                _logger.LogError(e,"Unhandled error");
                throw;
            }
        }

        public async Task<List<TemplateDto>> GetTemplates(GetTemplateKafkaRequest getTemplateRequest)
        {
            try
            {
                Guid messageId = Guid.NewGuid();
                if(await _kafkaService.Produce("imageRequestsTopic",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key = messageId.ToString(),
                    Value = JsonConvert.SerializeObject(getTemplateRequest),
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("getTemplates")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var templateDto = await _kafkaService.Consume<List<TemplateDto>>("imageResponsesTopic", messageId, "getTemplates");
                    _logger.LogInformation("Templates fetched successfully, Templates: {Templates}", JsonConvert.SerializeObject(getTemplateRequest));
                    return templateDto;
                }
                _logger.LogError("Error fetching templates, Templates: {Templates}", JsonConvert.SerializeObject(getTemplateRequest));
                throw new TemplateNotFoundException("Error fetching Templates, Templates: "+JsonConvert.SerializeObject(getTemplateRequest));
            }
            catch (Exception e)
            {
                if (e is not MyKafkaException)
                {
                    _logger.LogError(e,"Error fetching templates, Templates: {Templates}", JsonConvert.SerializeObject(getTemplateRequest));
                    throw new TemplateNotFoundException("Error fetching Templates, Templates: "+JsonConvert.SerializeObject(getTemplateRequest),e);
                }
                _logger.LogError(e,"Unhandled error");
                throw;
            }
        }

        public async Task<TemplateDto> UpdateTemplate(UpdateTemplateKafkaRequest modifyTemplateRequest)
        {
            try
            {
                Guid messageId = Guid.NewGuid();
                if(await _kafkaService.Produce("imageRequestsTopic",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key = messageId.ToString(),
                    Value = JsonConvert.SerializeObject(modifyTemplateRequest),
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("updateTemplate")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var templateDto = await _kafkaService.Consume<TemplateDto>("imageResponsesTopic", messageId, "updateTemplate");
                    _logger.LogInformation("Template updated successfully, Template: {Template}", JsonConvert.SerializeObject(modifyTemplateRequest));
                    return templateDto;
                }
                _logger.LogError("Error updating template, Template: {Template}", JsonConvert.SerializeObject(modifyTemplateRequest));
                throw new UpdateTemplateException("Error updating template, Template: "+JsonConvert.SerializeObject(modifyTemplateRequest));
            }
            catch (Exception e) 
            {
                if (e is not MyKafkaException)
                {
                    _logger.LogError(e,"Error updating template, Template: {Template}", JsonConvert.SerializeObject(modifyTemplateRequest));
                    throw new UpdateTemplateException("Error updating template, Template: "+JsonConvert.SerializeObject(modifyTemplateRequest),e);
                }
                _logger.LogError(e,"Unhandled error");
                throw;
            }
        }
    }
}