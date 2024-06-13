using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageAgregationService.Models
{
    public class ImageModel
    {
        [Key]
        public Guid Id {get;set;}
        [Required]
        public string Name {get;set;} = null!;
        [Required]
        public string Url {get;set;} = null!;
        [Required]
        public string Prompt {get;set;} = null!;
        public Guid TemplateId {get;set;}
        [ForeignKey("TemplateId")]
        public TemplateModel Template {get;set;} = null!;
        public Guid MarkId {get;set;}
        [ForeignKey("MarkId")]
        public MarkModel Mark {get;set;} = null!;
    }
}