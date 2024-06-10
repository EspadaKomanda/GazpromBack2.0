namespace ImageAgregationService.Exceptions.S3ServiceExceptions
{
    [System.Serializable]
    public class DeleteImageException : System.Exception
    {
        public DeleteImageException() { }
        public DeleteImageException(string message) : base(message) { }
        public DeleteImageException(string message, System.Exception inner) : base(message, inner) { }
        protected DeleteImageException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}