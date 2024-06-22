namespace UserService.Exceptions.RoleExceptions;

[System.Serializable]
public class GetRolesException : System.Exception
{
    public GetRolesException() { }
    public GetRolesException(string message) : base(message) { }
    public GetRolesException(string message, System.Exception inner) : base(message, inner) { }
}
