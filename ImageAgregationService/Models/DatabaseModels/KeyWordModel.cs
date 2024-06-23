using System.ComponentModel.DataAnnotations;

namespace ImageAgregationService.Models
{
    public class KeyWordModel
    {
        [Key]
        public Guid Id {get;set;}
        public string Word {get;set;}
    }
}