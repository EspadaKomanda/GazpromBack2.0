using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Roles.Requests;

public class RoleDeleteRequest
{
    [Required]
    public long Id { get; set; }
}