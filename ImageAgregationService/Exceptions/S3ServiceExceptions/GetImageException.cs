namespace ImageAgregationService.Exceptions.S3ServiceExceptions
{
    [System.Serializable]
    public class GetImageException : System.Exception
    {
        public GetImageException() { }
        public GetImageException(string message) : base(message) { }
        public GetImageException(string message, System.Exception inner) : base(message, inner) { }
    }
}