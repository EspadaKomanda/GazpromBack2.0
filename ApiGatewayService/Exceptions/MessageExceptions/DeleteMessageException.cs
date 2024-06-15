namespace ApiGatewayService.Exceptions.MessageExceptions
{
   [System.Serializable]
   public class DeleteMessageException : System.Exception
   {
    public DeleteMessageException() { }
    public DeleteMessageException(string message) : base(message) { }
    public DeleteMessageException(string message, System.Exception inner) : base(message, inner) { }
    protected DeleteMessageException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
   }
}