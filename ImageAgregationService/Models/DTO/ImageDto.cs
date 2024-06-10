namespace ImageAgregationService.Models.DTO
{
    public class ImageDto
    {
        public string Name {get;set;}
        public string Url {get;set;}
        public MarkDto mark {get;set;}
        public TemplateDto template {get;set;}
    }
}