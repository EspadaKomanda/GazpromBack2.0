namespace AuthService.Exceptions.User;

[Serializable]
public class UserInvalidPasswordException : Exception
{
    public UserInvalidPasswordException() { }
    public UserInvalidPasswordException(string message) : base(message) { }
    public UserInvalidPasswordException(string message, Exception inner) : base(message, inner) { }
}