namespace UserService.Exceptions.UserProfileExceptions.OwnerAssignmentException;

[Serializable]
public class OwnerAssignmentException : Exception
{
    public OwnerAssignmentException() { }
    public OwnerAssignmentException(string message) : base(message) { }
    public OwnerAssignmentException(string message, Exception inner) : base(message, inner) { }
}