using UserService.Models.Account.Requests;

namespace UserService.Services.Account;

public interface IAccountService
{
    public Task<bool> AccountRegister(AccountRegisterRequest request);
    public Task<bool> AccountFinishRegistration(AccountFinishRegistrationRequest request);
    public Task<bool> AccountLogin(AccountLoginRequest request);
    public Task<bool> AccountChangePassword(string username, AccountChangePasswordRequest request);
}