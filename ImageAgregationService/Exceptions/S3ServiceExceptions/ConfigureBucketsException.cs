namespace ImageAgregationService.Exceptions.S3ServiceExceptions
{
    public class ConfigureBucketsException : System.Exception
    {
        public ConfigureBucketsException() {}
        public ConfigureBucketsException(string message) : base(message) {}
        public ConfigureBucketsException(string message, System.Exception inner) : base(message, inner) {}
        public ConfigureBucketsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {}
    }
}