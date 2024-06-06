using System.ComponentModel.DataAnnotations;

namespace BackGazprom.Models.Account.Requests;

public class AccountChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; } = null!;
    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = null!;
}