using AuthService.Models.Roles.Requests;
using AuthService.Models.Roles.Responses;
using AuthService.Repositories;

namespace AuthService.Services.Roles;

public class RolesService(RoleRepository roleRepo) : IRolesService
{
    private readonly RoleRepository _roleRepo = roleRepo;
    public Task<RoleResponse> CreateRole(RoleCreateRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteRole(RoleDeleteRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<List<RoleResponse>> GetRoles()
    {
        throw new NotImplementedException();
    }

    public Task<RoleResponse> UpdateRole(RoleUpdateRequest request)
    {
        throw new NotImplementedException();
    }
}
