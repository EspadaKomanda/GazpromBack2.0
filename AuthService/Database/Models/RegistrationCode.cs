using System.ComponentModel.DataAnnotations;
using BackGazprom.Validation.Attributes;

namespace BackGazprom.Database.Models;

public class RegistrationCode 
{
    [Key]
    public long Id { get; set; }
    [Required]
    [ValidGuid]
    public string Code { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public DateTime ExpirationDate { get; set; }
}