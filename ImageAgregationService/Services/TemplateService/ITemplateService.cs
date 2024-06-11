using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;

namespace ImageAgregationService.Services.TemplateService
{
    public interface ITemplateService
    {
        Task<List<TemplateDto>> GetTemplates(GetTemplateRequest getTemplateRequest);
    }
}