using BackGazprom.Database.Models;

namespace BackGazprom.Repositories;

public interface IUserProfileRepository
{
    public IQueryable<UserProfile> GetUserProfiles();
    public Task<UserProfile?> GetUserProfileById(long id);
    public Task<UserProfile?> GetUserProfileByUsername(string username);
    public Task<bool> CreateUserProfile(UserProfile profile);
    public Task<bool> UpdateUserProfile(UserProfile profile);
    public Task<bool> DeleteUserProfile(UserProfile profile);
    public Task<bool> Save();
}