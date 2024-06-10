using System.ComponentModel.DataAnnotations;

namespace ImageAgregationService.Models
{
    public class TemplateModel
    {
        [Key]
        public int Id {get;set;}
        public string Name {get;set;}
        public string DefaultPrompt {get;set;}
    }
}