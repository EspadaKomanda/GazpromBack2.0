using System.ComponentModel.DataAnnotations;
using AuthService.Validation.Attributes;

namespace AuthService.Models.Account.Requests;

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
    [ValidAbout]
    public string? About { get; set; }
    [Required]
    [ValidPassword]
    public string Password { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string RegistrationCode { get; set; } = null!;
}