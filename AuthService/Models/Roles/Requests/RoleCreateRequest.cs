using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Roles.Requests;

public class RoleCreateRequest 
{
    [Required]
    public string Name { get; set; } = null!;
}