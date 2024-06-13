using AuthService.Database.Models;
using AuthService.Models.Account.Requests;

namespace AuthService.Services.UserService;

public interface IUserService
{
    public Task<bool> Login(AccountLoginRequest request);
    public Task<bool> ChangePassword(string username, AccountChangePasswordRequest request);
    public Task<User?> GetUserByUsername(string username);
    public Task<User?> GetUserByEmail(string email);
    public Task<User?> GetUserById(long id);
}