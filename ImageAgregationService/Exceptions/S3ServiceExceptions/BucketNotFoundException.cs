namespace ImageAgregationService.Exceptions.S3ServiceExceptions
{
    [System.Serializable]
    public class BucketNotFoundException : System.Exception
    {
        public BucketNotFoundException() { }
        public BucketNotFoundException(string message) : base(message) { }
        public BucketNotFoundException(string message, System.Exception inner) : base(message, inner) { }
    }
}