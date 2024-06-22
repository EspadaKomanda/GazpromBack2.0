using UserService.Database.Models;
using UserService.Exceptions.UserProfileExceptions.OwnerAssignmentException;
using UserService.Exceptions.UserProfileExceptions.ProfileNotFoundException;
using UserService.Models.UserProfile.Requests;
using UserService.Repositories;

namespace UserService.Services.UserProfileService;

public class UserProfileService(IUserProfileRepository userProfileRepo, IUserRepository userRepo, ILogger<UserProfileService> logger) : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepo = userProfileRepo;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly ILogger<UserProfileService> _logger = logger;

    public async Task<UserProfile> CreateUserProfile(CreateUserProfileRequest request)
    {
        var ownerUser = await _userRepo.GetUserById(request.UserId);

        // Check if user exists
        if (ownerUser == null)
        {
            _logger.LogError("User with ID {UserId} does not exist, therefore profile could not be created", request.UserId);
            throw new OwnerAssignmentException($"User with ID {request.UserId} does not exist, therefore profile could not be created");
        }

        var userProfile = new UserProfile
        {
            User = ownerUser,
            FirstName = request.FirstName,
            LastName = request.LastName,
            About = request.About
        };

        try
        {
            await _userProfileRepo.CreateUserProfile(userProfile);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to add user profile to database: {Error}", e.Message);
            throw;
        }

        return userProfile;
    }

    public async Task<bool> DeleteUserProfile(DeleteUserProfileByUserIdRequest request)
    {
        var userProfile = await _userProfileRepo.GetUserProfileById(request.Id);

        if (userProfile == null)
        {
            _logger.LogError("User profile of user with ID {Id} does not exist, cannot delete", request.Id);
            throw new UserProfileNotFoundException($"User profile of user with ID {request.Id} does not exist, cannot delete");
        }

        try 
        {
            var result = await _userProfileRepo.DeleteUserProfile(userProfile);

            if (!result)
            {
                _logger.LogError("Failed to delete user profile of user with ID {Id}. Database context returned false", request.Id);
                throw new Exception($"Failed to delete user profile of user with ID {request.Id}. Database context returned false");
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to delete user profile from database: {Error}", e.Message);
            throw;
        }
    }

    public async Task<UserProfile?> GetUserProfileById(GetUserProfileRequest request)
    {
        var userProfile = await _userProfileRepo.GetUserProfileById(request.Id);
        return userProfile;
    }

    public async Task<UserProfile> UpdateUserProfile(UpdateUserProfileRequest request)
    {
        var registeredProfile = await _userProfileRepo.GetUserProfileById(request.Id);

        if (registeredProfile == null)
        {
            _logger.LogError("User profile of user with ID {Id} does not exist, cannot update", request.Id);
            throw new UserProfileNotFoundException($"User profile of user with ID {request.Id} does not exist, cannot update");
        }

        registeredProfile.FirstName = request.FirstName ?? registeredProfile.FirstName;
        registeredProfile.LastName = request.LastName ?? registeredProfile.LastName;
        registeredProfile.About = request.About ?? registeredProfile.About;

        try
        {
            await _userProfileRepo.UpdateUserProfile(registeredProfile);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to update user profile in database: {Error}", e.Message);
            throw;
        }

        return registeredProfile;
    }
}
