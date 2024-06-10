namespace ImageAgregationService.Exceptions.GenerateImageExceptions
{
    [System.Serializable]
    public class GenerateImageException : System.Exception
    {
        public GenerateImageException() { }
        public GenerateImageException(string message) : base(message) { }
        public GenerateImageException(string message, System.Exception inner) : base(message, inner) { }
        protected GenerateImageException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}