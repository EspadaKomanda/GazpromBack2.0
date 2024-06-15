using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Roles.Requests;

public class RoleCreateRequest 
{
    [Required]
    public string Name { get; set; } = null!;
}