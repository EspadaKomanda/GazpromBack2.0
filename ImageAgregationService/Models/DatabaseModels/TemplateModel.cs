using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ImageAgregationService.Models
{
    [Index(nameof(Name), IsUnique = true)]

    public class TemplateModel
    {
        [Key]
        public Guid Guid {get;set;}
        public string Name {get;set;} = null!;
        public string DefaultPrompt {get;set;} = null!;
    }
}