using System.ComponentModel.DataAnnotations;

namespace UserService.Models.UserProfile.Requests;

public class CreateUserProfileRequest
{
    [Required]
    public long UserId {get;set;}
    [Required]
    public string FirstName {get;set;} = null!;
    [Required]
    public string LastName {get;set;} = null!;
    public string? About {get;set;}
}