using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageAgregationService.Models
{
    public class ImageModel
    {
        [Key]
        public Guid Id {get;set;}
        [Required]
        public string Name {get;set;}
        [Required]
        public string Url {get;set;}
        [Required]
        public string Prompt {get;set;}
        public Guid TemplateId {get;set;}
        [ForeignKey("TemplateId")]
        public TemplateModel Template {get;set;}
        public Guid MarkId {get;set;}
        [ForeignKey("MarkId")]
        public MarkModel Mark {get;set;}
    }
}