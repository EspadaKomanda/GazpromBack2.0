namespace ImageAgregationService.Exceptions.ConfigExceptions
{
    public class GetConfigException : Exception
    {
        public GetConfigException() {}
        public GetConfigException(string message) : base(message) {}
        public GetConfigException(string message, Exception inner) : base(message, inner) {}
    }
}