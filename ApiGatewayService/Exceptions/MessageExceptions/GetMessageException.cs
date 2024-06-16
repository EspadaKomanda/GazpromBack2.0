namespace ApiGatewayService.Exceptions.MessageExceptions
{
    [System.Serializable]
    public class GetMessageException : System.Exception
    {
        public GetMessageException() { }
        public GetMessageException(string message) : base(message) { }
        public GetMessageException(string message, System.Exception inner) : base(message, inner) { }
        protected GetMessageException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}