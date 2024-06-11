using ImageAgregationService.Exceptions.TemplateExceptions;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using ImageAgregationService.Repository;

namespace ImageAgregationService.Services.TemplateService
{
    public class TemplateService : ITemplateService
    {
        private readonly ILogger<TemplateService> _logger;
        private readonly ITemplateRepository _templateRepository;
        public TemplateService(ILogger<TemplateService> logger, ITemplateRepository templateRepository)
        {
            _logger = logger;
            _templateRepository = templateRepository;
        }
        public async Task<List<TemplateDto>> GetTemplates(GetTemplateKafkaRequest getTemplateRequest)
        {
            try
            {
                List<TemplateDto> templateDtos = new List<TemplateDto>();
                foreach (var templateName in getTemplateRequest.TemplateNames)
                {
                    var template = await _templateRepository.GetTemplateByName(templateName);
                    if(template == null)
                    {
                        _logger.LogError("Template not found! Template name: " + templateName);
                        throw new TemplateNotFoundException("Template not found! Template name: " + templateName);
                    }
                    templateDtos.Add(new TemplateDto
                    {
                        Name = template.Name
                    });
                }
                return templateDtos;
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get template!");
                throw new GetTemplateException("Failed to get template!", ex);
            }
        }
    }
}