using UserService.Database.Models;
using UserService.Repositories;

namespace UserService.Services.UserInfoService;

public class UserInfoService(IUserRepository userRepo) : IUserInfoService
{
    // TODO: Add implementation
    private readonly IUserRepository _userRepo = userRepo;
    public async Task<bool> CreateUser(User user)
    {
        return await _userRepo.CreateUser(user);
    }

    public Task<bool> DeleteUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserById(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _userRepo.GetUserByUsername(username);
    }

    public Task<bool> UpdateUser(User user)
    {
        throw new NotImplementedException();
    }
}
