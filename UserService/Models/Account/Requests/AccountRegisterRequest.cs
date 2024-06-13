using System.ComponentModel.DataAnnotations;
using UserService.Validation.Attributes;

namespace UserService.Models.Account.Requests;

public class AccountRegisterRequest
{
    [Required]
    [EmailAddress]
    [ValidEmailDomain]
    public string Email { get; set; } = null!;
}