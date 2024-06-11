namespace AuthService.Models.Roles.Responses;

public class RoleResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Protected { get; set; }
}