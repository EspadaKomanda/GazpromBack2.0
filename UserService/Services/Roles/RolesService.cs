using UserService.Exceptions.RoleExceptions;
using UserService.Models.Roles.Requests;
using UserService.Models.Roles.Responses;
using UserService.Repositories;

namespace UserService.Services.Roles;

public class RolesService(IRoleRepository roleRepo, ILogger<RolesService> logger) : IRolesService
{
    private readonly IRoleRepository _roleRepo = roleRepo;
    private readonly ILogger<RolesService> _logger = logger;
    
    public async Task<RoleResponse> CreateRole(RoleCreateRequest request)
    {
        // Check if role already exists
        if (await _roleRepo.GetRoleByName(request.Name) != null)
        {
            throw new CreateRoleException("Role already exists");
        }

        var newRole = new Role
        {
            Name = request.Name
        };

        try
        {
            // Attempt to create role
            if (await _roleRepo.CreateRole(newRole))
            {
                _logger.LogInformation("Created role {Name}", newRole.Name);

                // Return created role
                return new RoleResponse
                {
                    Id = newRole.Id,
                    Name = newRole.Name
                };
            }
        }
        catch (Exception err)
        {
            _logger.LogError("Failed to create role {Name}: {Error}", newRole.Name, err);
            throw;
        }
        throw new CreateRoleException("Failed to create role");
    }

    public async Task<bool> DeleteRole(RoleDeleteRequest request)
    {
        var role = await _roleRepo.GetRoleById(request.Id);

        // Check if role exists
        if (role == null)
        {
            throw new DeleteRoleException("Role does not exist");
        }

        // Make sure the role is not protected
        if (role.Protected ?? false)
        {
            throw new DeleteRoleException("The following role is protected");
        }

        try
        {
            // Delete the role
            var isSuccess = await _roleRepo.DeleteRole(role);

            // Make sure the role is deleted
            if (isSuccess)
            {
                _logger.LogInformation("Deleted role {Name}", role.Name);
                return true;
            }
        }
        catch (Exception err)
        {
            _logger.LogError("Failed to delete role {Name}: {Error}", role.Name, err);
            throw;
        }
        throw new DeleteRoleException("Failed to delete role");
    }

    public List<RoleResponse> GetRoles()
    {
        // Get all roles
        try
        {
            // FIXME: make sure this works asynchronously
            var roles = _roleRepo.GetRoles()
            .Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            return roles;
        }
        catch (Exception err)
        {
            _logger.LogError("Failed to get roles: {Error}", err);
            throw;
        }
    }

    public async Task<RoleResponse> UpdateRole(RoleUpdateRequest request)
    {
        var role = await _roleRepo.GetRoleById(request.Id);

        // Check if role exists
        if (role == null)
        {
            throw new DeleteRoleException("Role does not exist");
        }
        var initialName = role.Name;

        // Make sure the role is not protected
        if (role.Protected ?? false)
        {
            throw new DeleteRoleException("The following role is protected");
        }

        try
        {
            // Delete the role
            var isSuccess = await _roleRepo.UpdateRole(role);

            // Make sure the role is updated
            if (isSuccess)
            {
                _logger.LogInformation("Updated role {InitialName} {Name}", initialName, role.Name);
                var response = new RoleResponse
                {
                    Id = role.Id,
                    Name = role.Name
                };
                return response;
            }
        }
        catch (Exception err)
        {
            _logger.LogError("Failed to delete role {Name}: {Error}", role.Name, err);
            throw;
        }
        throw new DeleteRoleException("Failed to delete role");
    }
}
