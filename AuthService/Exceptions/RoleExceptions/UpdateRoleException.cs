namespace AuthService.Exceptions.RoleExceptions;

[Serializable]
public class UpdateRoleException : Exception
{
    public UpdateRoleException() { }
    public UpdateRoleException(string message) : base(message) { }
    public UpdateRoleException(string message, Exception inner) : base(message, inner) { }
}