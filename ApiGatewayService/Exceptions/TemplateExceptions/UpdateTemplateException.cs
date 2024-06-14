namespace ImageAgregationService.Exceptions.TemplateExceptions
{
   [Serializable]
   public class UpdateTemplateException : Exception
    {
    public UpdateTemplateException() { }
    public UpdateTemplateException(string message) : base(message) { }
    public UpdateTemplateException(string message, Exception inner) : base(message, inner) { }
    }
}