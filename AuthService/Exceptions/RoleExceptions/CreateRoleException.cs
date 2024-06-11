namespace AuthService.Exceptions.RoleExceptions;

[Serializable]
public class CreateRoleException : Exception
{
    public CreateRoleException() { }
    public CreateRoleException(string message) : base(message) { }
    public CreateRoleException(string message, Exception inner) : base(message, inner) { }
    protected CreateRoleException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}