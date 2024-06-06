using System.ComponentModel.DataAnnotations;

namespace BackGazprom.Models.Account.Requests;

public class AccountRegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}