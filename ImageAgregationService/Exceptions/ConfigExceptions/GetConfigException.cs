namespace ImageAgregationService.Exceptions.ConfigExceptions
{
    public class GetConfigException : System.Exception
    {
        public GetConfigException() {}
        public GetConfigException(string message) : base(message) {}
        public GetConfigException(string message, System.Exception inner) : base(message, inner) {}
        public GetConfigException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {}
    }
}