using BackGazprom.Database;
using BackGazprom.Database.Models;

namespace BackGazprom.Repositories;

public class UserRepository(ApplicationContext db) : IUserRepository
{
    private readonly ApplicationContext _db = db;

    public bool CreateUser(User user)
    {
        _db.Users.Add(user);
        return Save();
    }

    public bool UpdateUser(User user)
    {
        _db.Users.Update(user);
        return Save();
    }

    public bool DeleteUser(User user)
    {
        _db.Users.Remove(user);
        return Save();
    }

    public User? GetUserById(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public User? GetUserByUsername(string username)
    {
        return _db.Users.FirstOrDefault(u => u.Username == username);
    }

    public User? GetUserByEmail(string email)
    {
        return _db.Users.FirstOrDefault(u => u.Email == email);
    }

    public IQueryable<User> GetUsers()
    {
        return _db.Users.AsQueryable();
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }
}