namespace ImageAgregationService.Exceptions.TemplateExceptions
{
   [System.Serializable]
   public class AddTemplateException : System.Exception
   {
    public AddTemplateException() { }
    public AddTemplateException(string message) : base(message) { }
    public AddTemplateException(string message, System.Exception inner) : base(message, inner) { }
    protected AddTemplateException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
   }
}