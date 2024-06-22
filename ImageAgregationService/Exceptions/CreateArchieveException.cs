public class CreateArchieveException : System.Exception
{
    public CreateArchieveException() {}
    public CreateArchieveException(string message) : base(message) {}
    public CreateArchieveException(string message, System.Exception inner) : base(message, inner) {}
    public CreateArchieveException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {}
}