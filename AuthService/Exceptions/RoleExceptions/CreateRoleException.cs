namespace AuthService.Exceptions.RoleExceptions;

[Serializable]
public class CreateRoleException : Exception
{
    public CreateRoleException() { }
    public CreateRoleException(string message) : base(message) { }
    public CreateRoleException(string message, Exception inner) : base(message, inner) { }
}