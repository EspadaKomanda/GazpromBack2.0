using System.ComponentModel.DataAnnotations;
using UserService.Validation.Attributes;

namespace UserService.Database.Models;

public class UserProfile
{
    [Key]
    public long Id {get;set;}
    [Required]
    public User User {get;set;} = null!;
    [Required]
    [ValidName]
    public string FirstName {get;set;} = null!;
    [Required]
    [ValidName]
    public string LastName {get;set;} = null!;
    [ValidAbout]
    public string? About {get;set;}
}