namespace ImageAgregationService.Exceptions.S3ServiceExceptions
{
    [System.Serializable]
    public class StorageUnavailibleException : System.Exception
    {
        public StorageUnavailibleException() { }
        public StorageUnavailibleException(string message) : base(message) { }
        public StorageUnavailibleException(string message, System.Exception inner) : base(message, inner) { }
    }
}