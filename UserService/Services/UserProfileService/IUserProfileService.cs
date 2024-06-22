using UserService.Database.Models;
using UserService.Models.UserProfile.Requests;

namespace UserService.Services.UserProfileService;

// TODO: review documentation
public interface IUserProfileService
{
    /// <summary>
    /// Creates a user profile in the database.
    /// </summary>
    /// <param name="request">The user profile to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    Task<UserProfile> CreateUserProfile(CreateUserProfileRequest request);

    /// <summary>
    /// Updates a user profile in the database.
    /// </summary>
    /// <param name="request">The user profile to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    Task<UserProfile> UpdateUserProfile(UpdateUserProfileRequest request);

    /// <summary>
    /// Deletes a user profile from the database.
    /// </summary>
    /// <param name="request">The user profile to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    Task<bool> DeleteUserProfile(DeleteUserProfileByUserIdRequest request);

    /// <summary>
    /// Gets a user profile from the database by its ID.
    /// </summary>
    /// <param name="request">The ID of the user profile.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user profile, or null if it does not exist.</returns>
    Task<UserProfile?> GetUserProfileById(GetUserProfileRequest request);
}