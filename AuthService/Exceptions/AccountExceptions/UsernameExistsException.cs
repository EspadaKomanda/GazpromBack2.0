namespace AuthService.Exceptions.AccountExceptions;

[Serializable]
public class UsernameExistsException : Exception
{
    public UsernameExistsException() { }
    public UsernameExistsException(string message) : base(message) { }
    public UsernameExistsException(string message, Exception inner) : base(message, inner) { }
}