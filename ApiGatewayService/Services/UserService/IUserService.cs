using AuthService.Database.Models;

namespace UserService.Services.UserInfoService;

public interface IUserService
{
    public Task<User?> GetUserByUsername(string username);
}