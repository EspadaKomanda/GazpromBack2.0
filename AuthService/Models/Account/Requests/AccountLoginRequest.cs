using System.ComponentModel.DataAnnotations;

namespace BackGazprom.Models.Account.Requests;

public class AccountLoginRequest
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}