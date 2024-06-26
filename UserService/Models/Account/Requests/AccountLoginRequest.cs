using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Account.Requests;

public class AccountLoginRequest
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}