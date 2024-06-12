using ImageAgregationService.Exceptions.TemplateExceptions;
using ImageAgregationService.Models;
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

        public async Task<bool> AddTemplate(TemplateDto templateDto)
        {
            try
            {
                return await _templateRepository.CreateTemplate(new TemplateModel
                {
                    Name = templateDto.Name,
                    DefaultPrompt = templateDto.DefaultPrompt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add template!");
                throw new AddTemplateException("Failed to add template!", ex);
            }
        }

        public async Task<bool> DeleteTemplate(DeleteTemplateKafkaRequest deleteTemplateRequest)
        {
            try
            {
                return await _templateRepository.DeleteTemplate(await _templateRepository.GetTemplateByName(deleteTemplateRequest.Name));
                 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete template!");
                throw new DeleteTemplateException("Failed to delete template!", ex);
            }
        }

        public async Task<List<TemplateDto>> GetTemplates(GetTemplateKafkaRequest getTemplateRequest)
        {
            try
            {
                List<TemplateDto> templateDtos = new List<TemplateDto>();
                foreach (var templateName in getTemplateRequest.TemplateNames)
                {
                    var template = await _templateRepository.GetTemplateByName(templateName.Name);
                    if(template == null)
                    {
                        _logger.LogError("Template not found! Template name: " + templateName);
                        throw new TemplateNotFoundException("Template not found! Template name: " + templateName);
                    }
                    templateDtos.Add(new TemplateDto
                    {
                        Name = template.Name,
                        DefaultPrompt = template.DefaultPrompt
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

        public async Task<TemplateDto> UpdateTemplate(UpdateTemplateKafkaRequest modifyTemplateRequest)
        {
            try
            {
                var template = await _templateRepository.GetTemplateByName(modifyTemplateRequest.OldName);
                template.DefaultPrompt = modifyTemplateRequest.NewTemplate.DefaultPrompt;
                template.Name = modifyTemplateRequest.NewTemplate.Name; 
                if(await _templateRepository.UpdateTemplate(template))
                {
                    return new TemplateDto
                    {
                        Name = template.Name,
                        DefaultPrompt = template.DefaultPrompt
                    };
                }
                else
                {
                    _logger.LogError("Failed to update template!");
                    throw new UpdateTemplateException("Failed to update template!");
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled update exception!");
                throw new UpdateTemplateException("Unhandled update exception!", ex);
            }
        }
    }
}