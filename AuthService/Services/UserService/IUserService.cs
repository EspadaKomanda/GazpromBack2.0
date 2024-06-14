using AuthService.Database.Models;
using AuthService.Models.Account.Requests;

namespace AuthService.Services.UserService;

public interface IUserService
{
    public Task<bool> Login(AccountLoginRequest request);
    public Task<User?> GetUserByUsername(string username);
}