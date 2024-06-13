using System.ComponentModel.DataAnnotations;
using UserService.Validation.Attributes;

namespace UserService.Models.Roles.Requests;

public class RoleUpdateRequest
{
    [Required]
    public long Id { get; set; }
    [Required]
    [ValidRole]
    public string Name { get; set; } = null!;
}