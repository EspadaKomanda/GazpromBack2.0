using System.ComponentModel.DataAnnotations;

namespace UserService.Models.UserProfile.Requests;

// TODO: rename the method and file to mention that it uses the user id
public class DeleteUserProfileRequest
{
    [Required]
    public long Id {get;set;}
}