using System.ComponentModel.DataAnnotations;

namespace ImageAgregationService.Models
{
    public class MarkModel
    {
        [Key]
        public int Id {get;set;}
        
        [Required]
        public string Name {get;set;}
    }
}