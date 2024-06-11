namespace AuthService.Exceptions.RoleExceptions;

[Serializable]

public class DeleteRoleException : Exception
{
    public DeleteRoleException() { }
    public DeleteRoleException(string message) : base(message) { }
    public DeleteRoleException(string message, Exception inner) : base(message, inner) { }
    protected DeleteRoleException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
