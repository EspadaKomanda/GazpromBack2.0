using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Models.Account.Requests;

public class AccountRefreshTokenRequest
{
    [Required]
    [FromHeader]
    public string Authorization { get; set; } = null!;
}