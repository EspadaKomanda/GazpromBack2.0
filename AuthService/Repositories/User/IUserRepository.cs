using BackGazprom.Database;
using BackGazprom.Database.Models;

namespace BackGazprom.Repositories;

public interface IUserRepository
{
    public IQueryable<User> GetUsers();
    public Task<User?> GetUserById(int id);
    public Task<User?> GetUserByUsername(string username);
    public Task<User?> GetUserByEmail(string email);
    public Task<bool> CreateUser(User user);
    public Task<bool> UpdateUser(User user);
    public Task<bool> DeleteUser(User user);
    public Task<bool> Save();
}