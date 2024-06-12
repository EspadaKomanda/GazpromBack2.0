namespace ImageAgregationService.Models.RequestModels
{
    public class GenerateImageKafkaRequest
    {
        public string TemplateName {get; set;}
        public string Text {get; set;}
    }
}