namespace AuthService.Exceptions.AccountExceptions;

[Serializable]
public class PasswordMatchException : Exception
{
    public PasswordMatchException() { }
    public PasswordMatchException(string message) : base(message) { }
    public PasswordMatchException(string message, Exception inner) : base(message, inner) { }
}