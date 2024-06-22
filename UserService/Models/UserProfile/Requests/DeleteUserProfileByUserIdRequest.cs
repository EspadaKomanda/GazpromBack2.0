using System.ComponentModel.DataAnnotations;

namespace UserService.Models.UserProfile.Requests;

public class DeleteUserProfileByUserIdRequest
{
    [Required]
    public long Id {get;set;}
}