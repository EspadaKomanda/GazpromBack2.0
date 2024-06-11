using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Roles.Requests;

public class RoleDeleteRequest
{
    [Required]
    public long Id { get; set; }
}