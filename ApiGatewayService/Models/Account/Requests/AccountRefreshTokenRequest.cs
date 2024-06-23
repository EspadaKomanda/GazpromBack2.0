using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Models.Account.Requests;

public class AccountRefreshTokenRequest
{
    [Required]
    [FromHeader]
    public string Authorization { get; set; } = null!;
}