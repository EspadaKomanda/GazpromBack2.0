namespace ApiGatewayService.Exceptions.User
{
    [System.Serializable]
    public class FinishRegistrationException : System.Exception
    {
        public FinishRegistrationException() { }
        public FinishRegistrationException(string message) : base(message) { }
        public FinishRegistrationException(string message, System.Exception inner) : base(message, inner) { }
        protected FinishRegistrationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}