using System.ComponentModel.DataAnnotations;
using BackGazprom.Validation.Attributes;

namespace BackGazprom.Models.Account.Requests;

public class AccountFinishRegistrationRequest
{
    [Required]
    [ValidUsername]
    public string Username { get; set; } = null!;
    [Required]
    [ValidName]
    public string FirstName { get; set; } = null!;
    [Required]
    [ValidName]
    public string LastName { get; set; } = null!;
    [Required]
    [ValidPassword]
    public string Password { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string RegistrationCode { get; set; } = null!;
}