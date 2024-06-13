namespace ImageAgregationService.Exceptions.GenerateImageExceptions
{
    [System.Serializable]
    public class GetImagesException : System.Exception
    {
        public GetImagesException() { }
        public GetImagesException(string message) : base(message) { }
        public GetImagesException(string message, System.Exception inner) : base(message, inner) { }
    }
}