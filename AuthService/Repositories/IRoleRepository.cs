namespace AuthService.Repositories;

public interface IRoleRepository
{
    public IQueryable<Role> GetRoles();
    public Task<Role?> GetRoleById(long id);
    public Task<Role?> GetRoleByName(string name);
    public Task<bool> CreateRole(Role role);
    public Task<bool> UpdateRole(Role role);
    public Task<bool> DeleteRole(Role role);
    public Task<bool> Save();
}