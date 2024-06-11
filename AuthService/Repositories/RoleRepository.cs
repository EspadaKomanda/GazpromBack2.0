
using AuthService.Database;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories;

public class RoleRepository(ApplicationContext db) : IRoleRepository
{
    private readonly ApplicationContext _db = db;
    public async Task<bool> CreateRole(Role role)
    {
        _db.Roles.Add(role);
        return await Save();
    }

    public async Task<bool> DeleteRole(Role role)
    {
        _db.Roles.Remove(role);
        return await Save();
    }

    public async Task<Role?> GetRoleById(long id)
    {
        return await _db.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role?> GetRoleByName(string name)
    {
        return await _db.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

    public IQueryable<Role> GetRoles()
    {
        return _db.Roles.AsQueryable();
    }

    public async Task<bool> Save()
    {
        return await _db.SaveChangesAsync() >= 0;
    }

    public async Task<bool> UpdateRole(Role role)
    {
        _db.Roles.Update(role);
        return await Save();
    }
}