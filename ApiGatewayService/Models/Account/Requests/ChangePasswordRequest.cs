using System.ComponentModel.DataAnnotations;
using BackGazprom.Validation.Attributes;

namespace BackGazprom.Models.Account.Requests;

public class AccountChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; } = null!;
    [Required]
    [ValidPassword]
    public string NewPassword { get; set; } = null!;
}