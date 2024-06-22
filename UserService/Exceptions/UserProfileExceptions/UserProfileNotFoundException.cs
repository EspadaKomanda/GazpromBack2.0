namespace UserService.Exceptions.UserProfileExceptions.ProfileNotFoundException;

[Serializable]
public class UserProfileNotFoundException : Exception
{
    public UserProfileNotFoundException() { }
    public UserProfileNotFoundException(string message) : base(message) { }
    public UserProfileNotFoundException(string message, Exception inner) : base(message, inner) { }
}