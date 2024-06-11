using System.ComponentModel.DataAnnotations;
using AuthService.Validation.Attributes;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Name), IsUnique = true)]
public class Role
{
    [Key]
    public long Id { get; set; }
    [Required]
    [ValidRole]
    public string Name { get; set; } = null!;
    public bool? Protected { get; set; } = false;
}