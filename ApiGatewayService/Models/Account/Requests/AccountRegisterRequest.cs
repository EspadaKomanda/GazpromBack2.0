using System.ComponentModel.DataAnnotations;
using BackGazprom.Validation.Attributes;

namespace BackGazprom.Models.Account.Requests;

public class AccountRegisterRequest
{
    [Required]
    [EmailAddress]
    [ValidEmailDomain]
    public string Email { get; set; } = null!;
}