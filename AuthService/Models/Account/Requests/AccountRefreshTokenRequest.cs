using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Account.Requests;

public class AccountRefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}