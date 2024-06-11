using System.ComponentModel.DataAnnotations;

namespace DialogService.Database.Models;

public class Dialog
{
    [Key]
    public long Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
}