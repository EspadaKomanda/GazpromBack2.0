namespace AuthService.Exceptions.AccountExceptions;

[Serializable]
public class RegistrationLimitException : Exception
{
    public RegistrationLimitException() { }
    public RegistrationLimitException(string message) : base(message) { }
    public RegistrationLimitException(string message, Exception inner) : base(message, inner) { }
}