using System.ComponentModel.DataAnnotations;
using AuthService.Validation.Attributes;

namespace AuthService.Models.Roles.Requests;

public class RoleUpdateRequest
{
    [Required]
    public long Id { get; set; }
    [Required]
    [ValidRole]
    public string Name { get; set; } = null!;
}