using System.ComponentModel.DataAnnotations;
using AuthService.Validation.Attributes;

namespace AuthService.Models.Account.Requests;

public class AccountChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; } = null!;
    [Required]
    [ValidPassword]
    public string NewPassword { get; set; } = null!;
}