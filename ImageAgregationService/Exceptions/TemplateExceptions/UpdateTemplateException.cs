namespace ImageAgregationService.Exceptions.TemplateExceptions
{
   [System.Serializable]
   public class UpdateTemplateException : System.Exception
   {
    public UpdateTemplateException() { }
    public UpdateTemplateException(string message) : base(message) { }
    public UpdateTemplateException(string message, System.Exception inner) : base(message, inner) { }
    protected UpdateTemplateException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}