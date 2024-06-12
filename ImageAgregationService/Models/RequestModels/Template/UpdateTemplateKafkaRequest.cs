using ImageAgregationService.Models.DTO;

namespace ImageAgregationService.Models.RequestModels
{
    public class UpdateTemplateKafkaRequest
    {
        public string OldName {get;set;}
        public TemplateDto NewTemplate {get;set;}
    }
}