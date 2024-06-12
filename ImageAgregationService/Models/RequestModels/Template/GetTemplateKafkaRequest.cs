using ImageAgregationService.Models.DTO;

namespace ImageAgregationService.Models.RequestModels
{
    public class GetTemplateKafkaRequest
    {
        public List<TemplateDto> TemplateNames {get;set;}
    }
}