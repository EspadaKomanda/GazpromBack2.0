using UserService.Models.Roles.Requests;
using UserService.Models.Roles.Responses;

namespace UserService.Services.Roles;

/// <summary>
/// Provides functionality for managing roles.
/// </summary>
public interface IRolesService
{
    /// <summary>
    /// Gets a list of all roles.
    /// </summary>
    /// <returns>A list of <see cref="RoleResponse"/> objects representing the roles.</returns>
    List<RoleResponse> GetRoles();

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="request">The <see cref="RoleCreateRequest"/> object containing the data for the new role.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="RoleResponse"/> object representing the created role.</returns>
    Task<RoleResponse> CreateRole(RoleCreateRequest request);

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    /// <param name="request">The <see cref="RoleUpdateRequest"/> object containing the data for the updated role.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="RoleResponse"/> object representing the updated role.</returns>
    Task<RoleResponse> UpdateRole(RoleUpdateRequest request);

    /// <summary>
    /// Deletes a role.
    /// </summary>
    /// <param name="request">The <see cref="RoleDeleteRequest"/> object containing the data for the role to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the operation was successful.</returns>
    Task<bool> DeleteRole(RoleDeleteRequest request);
}
