namespace UserService.Exceptions.RoleExceptions;

[Serializable]

public class DeleteRoleException : Exception
{
    public DeleteRoleException() { }
    public DeleteRoleException(string message) : base(message) { }
    public DeleteRoleException(string message, Exception inner) : base(message, inner) { }
}