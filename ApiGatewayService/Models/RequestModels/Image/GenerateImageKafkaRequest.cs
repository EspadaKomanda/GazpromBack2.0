namespace ImageAgregationService.Models.RequestModels
{
    public class GenerateImageKafkaRequest
    {
        public string TemplateName {get; set;} = null!;
        public string Text {get; set;} = null!;
    }
}