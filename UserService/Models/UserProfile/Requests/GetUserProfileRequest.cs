using System.ComponentModel.DataAnnotations;

namespace UserService.Models.UserProfile.Requests;

public class GetUserProfileRequest
{
    [Required]
    public long Id {get;set;}
}