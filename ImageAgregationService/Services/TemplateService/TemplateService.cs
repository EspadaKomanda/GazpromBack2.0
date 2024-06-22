using ImageAgregationService.Exceptions.TemplateExceptions;
using ImageAgregationService.Models;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using ImageAgregationService.Repository;
using ImageAgregationService.Repository.ImageRepository;
using ImageAgregationService.Repository.MarkRepository;

namespace ImageAgregationService.Services.TemplateService
{
    public class TemplateService(ILogger<TemplateService> logger, ITemplateRepository templateRepository, IImageRepository imageRepository, IMarkRepository markRepository, IS3Service s3Service) : ITemplateService
    {
        private readonly ILogger<TemplateService> _logger = logger;
        private readonly ITemplateRepository _templateRepository = templateRepository;
        private readonly IImageRepository _imageRepository = imageRepository;
        private readonly IMarkRepository _markRepository = markRepository;
        private readonly IS3Service _s3Service = s3Service;

        public async Task<bool> AddTemplate(TemplateDto templateDto)
        {
            try
            {
                if (await _templateRepository.GetTemplateByName(templateDto.Name) != null)
                {
                    _logger.LogError("Template already exists!");
                    throw new AddTemplateException("Template already exists!");
                }
                if(await _s3Service.CheckIfBucketExists(templateDto.Name))
                {
                    _logger.LogError("Bucket already exists!");
                    throw new AddTemplateException("Bucket already exists!");
                }
                Guid guid= Guid.NewGuid();
                if(!await _s3Service.CreateBucket(guid.ToString()))
                {
                    _logger.LogError("Failed to create bucket!");
                    throw new AddTemplateException("Failed to create bucket!");
                }

                return await _templateRepository.CreateTemplate(new TemplateModel
                {
                    Guid = guid,
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
                var template = await _templateRepository.GetTemplateByName(deleteTemplateRequest.Name);
                template.Availible = false;
                return await _templateRepository.UpdateTemplate(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete template!");
                throw new DeleteTemplateException("Failed to delete template!", ex);
            }
        }

        public async Task<List<TemplateDto>> GetTemplates()
        {
            try
            {
                List<TemplateDto> templateDtos = [];
                foreach (var template in  _templateRepository.GetTemplates())
                {
                    if(template.Availible)
                    {
                        templateDtos.Add(new TemplateDto
                        {
                            Name = template.Name,
                            DefaultPrompt = template.DefaultPrompt
                        });
                    }
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

                if (template == null)
                 {
                    _logger.LogError("Template not found! Template name: {Name}", modifyTemplateRequest.OldName);
                    throw new TemplateNotFoundException("Template not found! Template name: " + modifyTemplateRequest.OldName);
                }

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