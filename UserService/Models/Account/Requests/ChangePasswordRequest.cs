using System.ComponentModel.DataAnnotations;
using UserService.Validation.Attributes;

namespace UserService.Models.Account.Requests;

public class AccountChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; } = null!;
    [Required]
    [ValidPassword]
    public string NewPassword { get; set; } = null!;
}