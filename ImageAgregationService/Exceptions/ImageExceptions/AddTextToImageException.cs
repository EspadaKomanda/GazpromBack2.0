namespace ImageAgregationService.Exceptions.GenerateImageExceptions
{
   [System.Serializable]
   public class AddTextToImageException : System.Exception
   {
    public AddTextToImageException() { }
    public AddTextToImageException(string message) : base(message) { }
    public AddTextToImageException(string message, System.Exception inner) : base(message, inner) { }
   }
}