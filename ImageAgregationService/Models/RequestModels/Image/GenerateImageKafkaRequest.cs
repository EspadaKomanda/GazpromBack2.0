namespace ImageAgregationService.Models.RequestModels
{
    public class GenerateImageKafkaRequest
    {
        public string TemplateName {get; set;} = null!;
        public string Text {get; set;} = null!;
        public string ImageText{get; set;} = null!;
        public List<string> AllowedColors {get; set;} = null!;
        public Resolution Resolution {get; set;} = null!;
        public ResolutionPosition Position {get; set;} = null!;
        public string FontName {get; set;} = null!;
        public string Background {get; set;} = null!;
        public bool CheckColours {get; set;} = true;
    }
}