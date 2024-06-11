using AuthService.Models.Roles.Requests;
using AuthService.Models.Roles.Responses;

namespace AuthService.Services.Roles;

public interface IRolesService
{
    public List<RoleResponse> GetRoles();
    public Task<RoleResponse> CreateRole(RoleCreateRequest request);
    public Task<RoleResponse> UpdateRole(RoleUpdateRequest request);
    public Task<bool> DeleteRole(RoleDeleteRequest request);
}