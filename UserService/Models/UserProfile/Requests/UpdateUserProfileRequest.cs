using System.ComponentModel.DataAnnotations;

namespace UserService.Models.UserProfile.Requests;

public class UpdateUserProfileRequest
{
    [Required]
    public long Id {get;set;}
    public string? FirstName {get;set;}
    public string? LastName {get;set;}
    public string? About {get;set;}
}