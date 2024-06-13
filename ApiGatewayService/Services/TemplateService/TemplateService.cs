using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using KafkaTestLib.Kafka;

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
        public Task<bool> AddTemplate(TemplateDto templateDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTemplate(DeleteTemplateKafkaRequest deleteTemplateRequest)
        {
            throw new NotImplementedException();
        }

        public Task<List<TemplateDto>> GetTemplates(GetTemplateKafkaRequest getTemplateRequest)
        {
            throw new NotImplementedException();
        }

        public Task<TemplateDto> UpdateTemplate(UpdateTemplateKafkaRequest modifyTemplateRequest)
        {
            throw new NotImplementedException();
        }
    }
}