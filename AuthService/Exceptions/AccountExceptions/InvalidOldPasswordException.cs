namespace AuthService.Exceptions.AccountExceptions;

[Serializable]
public class InvalidOldPasswordException : Exception
{
    public InvalidOldPasswordException() { }
    public InvalidOldPasswordException(string message) : base(message) { }
    public InvalidOldPasswordException(string message, Exception inner) : base(message, inner) { }
}