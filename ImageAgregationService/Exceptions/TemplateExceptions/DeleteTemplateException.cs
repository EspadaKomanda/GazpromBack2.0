namespace ImageAgregationService.Exceptions.TemplateExceptions
{
    [System.Serializable]
    public class DeleteTemplateException : System.Exception
    {
        public DeleteTemplateException() { }
        public DeleteTemplateException(string message) : base(message) { }
        public DeleteTemplateException(string message, System.Exception inner) : base(message, inner) { }
        protected DeleteTemplateException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}