using UserService.Database;
using UserService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace UserService.Repositories;

public class UserProfileRepository(ApplicationContext db) : IUserProfileRepository
{
    private readonly ApplicationContext _db = db;

    public async Task<bool> CreateUserProfile(UserProfile profile)
    {
        _db.UserProfiles.Add(profile);
        return await Save();
    }

    public async Task<bool> DeleteUserProfile(UserProfile profile)
    {
        _db.UserProfiles.Remove(profile);
        return await Save();
    }

    public async Task<UserProfile?> GetUserProfileById(long id)
    {
        return await _db.UserProfiles.FirstOrDefaultAsync(up => up.Id == id);
    }

    public async Task<UserProfile?> GetUserProfileByUsername(string username)
    {
        return await _db.UserProfiles
        .Include(up => up.User)
        .FirstOrDefaultAsync(up => up.User.Username == username);
    }

    public IQueryable<UserProfile> GetUserProfiles()
    {
        return _db.UserProfiles;
    }

    public async Task<bool> Save()
    {
        return await _db.SaveChangesAsync() >= 0;
    }

    public async Task<bool> UpdateUserProfile(UserProfile profile)
    {
        _db.UserProfiles.Update(profile);
        return await Save();
    }
}