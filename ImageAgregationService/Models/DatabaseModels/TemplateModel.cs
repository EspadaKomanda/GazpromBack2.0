using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ImageAgregationService.Models
{
    [Index(nameof(Name), IsUnique = true)]

    public class TemplateModel
    {
        [Key]
        public int Id {get;set;}
        public string Name {get;set;}
        public string DefaultPrompt {get;set;}
    }
}