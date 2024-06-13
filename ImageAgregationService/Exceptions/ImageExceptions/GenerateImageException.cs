namespace ImageAgregationService.Exceptions.GenerateImageExceptions
{
    [System.Serializable]
    public class GenerateImageException : System.Exception
    {
        public GenerateImageException() { }
        public GenerateImageException(string message) : base(message) { }
        public GenerateImageException(string message, System.Exception inner) : base(message, inner) { }
    }
}