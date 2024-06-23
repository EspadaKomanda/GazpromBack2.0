namespace ImageAgregationService.Models.DTO
{
    public class ImageDto
    {
        public Guid Id {get;set;}
        public string Name {get;set;} = null!;
        public string Url {get;set;} = null!;
        public MarkDto Mark {get;set;} = null!;
        public TemplateDto Template {get;set;} = null!;
        public List<KeyWordDTO> KeyWords {get;set;}
    }
}