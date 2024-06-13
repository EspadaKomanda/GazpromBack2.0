namespace ImageAgregationService.Exceptions.S3ServiceExceptions
{
    public class ImageNotFoundException : System.Exception
    {
        public ImageNotFoundException() {}
        public ImageNotFoundException(string message) : base(message) {}
        public ImageNotFoundException(string message, System.Exception inner) : base(message, inner) {}
    }
}