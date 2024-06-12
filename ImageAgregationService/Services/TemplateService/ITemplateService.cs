using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;

namespace ImageAgregationService.Services.TemplateService
{
    public interface ITemplateService
    {
        Task<List<TemplateDto>> GetTemplates(GetTemplateKafkaRequest getTemplateRequest);
        Task<bool> AddTemplate(TemplateDto templateDto);
        Task<bool> DeleteTemplate(DeleteTemplateKafkaRequest deleteTemplateRequest);
        Task<TemplateDto> UpdateTemplate(UpdateTemplateKafkaRequest modifyTemplateRequest);
    }
}