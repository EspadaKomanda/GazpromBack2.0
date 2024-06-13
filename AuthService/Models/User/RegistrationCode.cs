using System.ComponentModel.DataAnnotations;

namespace AuthService.Database.Models;

public class RegistrationCode 
{
    [Key]
    public long Id { get; set; }
    [Required]
    public string Code { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public DateTime ExpirationDate { get; set; }
}