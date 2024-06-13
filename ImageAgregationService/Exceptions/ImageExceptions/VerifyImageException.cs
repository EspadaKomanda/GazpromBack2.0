namespace ImageAgregationService.Exceptions.GenerateImageExceptions
{
    [System.Serializable]
    public class VerifyImageException : System.Exception
    {
        public VerifyImageException() { }
        public VerifyImageException(string message) : base(message) { }
        public VerifyImageException(string message, System.Exception inner) : base(message, inner) { }
    }
}