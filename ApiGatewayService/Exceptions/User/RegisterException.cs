namespace ApiGatewayService.Exceptions.User
{
    [System.Serializable]
    public class RegisterException : System.Exception
    {
        public RegisterException() { }
        public RegisterException(string message) : base(message) { }
        public RegisterException(string message, System.Exception inner) : base(message, inner) { }
        protected RegisterException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}