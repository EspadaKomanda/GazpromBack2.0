using System.ComponentModel.DataAnnotations;

namespace AuthService.Database.Models;

public class UserProfile
{
    [Key]
    public long Id {get;set;}
    [Required]
    public User User {get;set;} = null!;
    [Required]
    public string FirstName {get;set;} = null!;
    [Required]
    public string LastName {get;set;} = null!;
    public string? About {get;set;}
}