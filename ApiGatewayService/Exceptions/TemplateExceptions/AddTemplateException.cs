namespace ImageAgregationService.Exceptions.TemplateExceptions
{
   [System.Serializable]
   public class AddTemplateException : System.Exception
   {
    public AddTemplateException() { }
    public AddTemplateException(string message) : base(message) { }
    public AddTemplateException(string message, System.Exception inner) : base(message, inner) { }
   }
}