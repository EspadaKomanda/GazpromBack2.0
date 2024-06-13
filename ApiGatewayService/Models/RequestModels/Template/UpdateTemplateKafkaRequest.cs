using ImageAgregationService.Models.DTO;

namespace ImageAgregationService.Models.RequestModels
{
    public class UpdateTemplateKafkaRequest
    {
        public string OldName {get;set;} = null!;
        public TemplateDto NewTemplate {get;set;} = null!;
    }
}