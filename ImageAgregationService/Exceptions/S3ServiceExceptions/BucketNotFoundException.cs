namespace ImageAgregationService.Exceptions.S3ServiceExceptions
{
    [System.Serializable]
    public class BucketNotFoundException : System.Exception
    {
        public BucketNotFoundException() { }
        public BucketNotFoundException(string message) : base(message) { }
        public BucketNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected BucketNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}