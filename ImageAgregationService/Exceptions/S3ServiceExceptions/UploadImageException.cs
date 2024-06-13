namespace ImageAgregationService.Exceptions.S3ServiceExceptions
{
    public class UploadImageException : System.Exception
    {
        public UploadImageException() {}
        public UploadImageException(string message) : base(message) {}
        public UploadImageException(string message, System.Exception inner) : base(message, inner) {}
    }
}