namespace ApiGatewayService.Exceptions.MessageExceptions
{
    public class SendMessageException : System.Exception
    {
        public SendMessageException() {}
        public SendMessageException(string message) : base(message) {}
        public SendMessageException(string message, System.Exception inner) : base(message, inner) {}
        public SendMessageException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {}
    }
}