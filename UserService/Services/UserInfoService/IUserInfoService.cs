using UserService.Database.Models;

namespace UserService.Services.UserInfoService;

public interface IUserInfoService
{
    public Task<User?> GetUserByUsername(string username);
    public Task<User?> GetUserByEmail(string email);
    public Task<User?> GetUserById(long id);
    public Task<bool> CreateUser(User user);
    public Task<bool> UpdateUser(User user);
    public Task<bool> DeleteUser(User user);
}