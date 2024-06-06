using BackGazprom.Database;
using BackGazprom.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BackGazprom.Repositories;

public class UserRepository(ApplicationContext db) : IUserRepository
{
    private readonly ApplicationContext _db = db;

    public async Task<bool> CreateUser(User user)
    {
        _db.Users.Add(user);
        return await Save();
    }

    public async Task<bool> UpdateUser(User user)
    {
        _db.Users.Update(user);
        return await Save();
    }

    public async Task<bool> DeleteUser(User user)
    {
        _db.Users.Remove(user);
        return await Save();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public IQueryable<User> GetUsers()
    {
        return _db.Users.AsQueryable();
    }

    public async Task<bool> Save()
    {
        return await _db.SaveChangesAsync() >= 0;
    }
}