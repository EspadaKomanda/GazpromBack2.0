namespace ImageAgregationService.Exceptions.MarkExceptions
{
    [System.Serializable]
    public class UpdateMarkException : System.Exception
    {
        public UpdateMarkException() { }
        public UpdateMarkException(string message) : base(message) { }
        public UpdateMarkException(string message, System.Exception inner) : base(message, inner) { }
    }
}