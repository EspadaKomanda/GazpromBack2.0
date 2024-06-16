using AuthService.Models.Account.Requests;

namespace UserService.Services.Account;

/// <summary>
/// Interface for account service.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>True if registration was successful, otherwise false or throws an exception.</returns>
    Task<bool> AccountRegister(AccountRegisterRequest request);

    /// <summary>
    /// Finishes the registration process for a user.
    /// </summary>
    /// <param name="request">The finish registration request.</param>
    /// <returns>True if registration was successful, otherwise false or throws an exception.</returns>
    Task<bool> AccountFinishRegistration(AccountFinishRegistrationRequest request);
    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <returns>True if login was successful, otherwise false or throws an exception.</returns>
   
    /// <summary>
    /// Changes the password of a user.
    /// </summary>
    /// <param name="request">The change password request.</param>
    /// <returns>True if password was changed successfully, otherwise false or throws an exception.</returns>
    Task<bool> AccountChangePassword(AccountChangePasswordRequest request);
}
