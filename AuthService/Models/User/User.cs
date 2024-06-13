using System.ComponentModel.DataAnnotations;

namespace AuthService.Database.Models;

public class User {
    [Key]
    public long Id {get;set;}
    [Required]
    public string Username {get;set;} = null!;
    [Required]
    [EmailAddress]
    public string Email {get;set;} = null!;
    [Required]
    public string Password {get;set;} = null!;
    [Required]
    public DateTime RegistrationDate {get;set;}
    [Required]
    public DateTime PasswordChangeDate {get;set;}
    public bool? IsSuper {get;set;}=false;
    public bool? ForcePasswordChange {get;set;}=false;
    [Required]
    public Role Role {get;set;} = null!;
    public long RoleId {get;set;}
}