namespace ImageAgregationService.Exceptions.TemplateExceptions
{
    [System.Serializable]
    public class DeleteTemplateException : System.Exception
    {
        public DeleteTemplateException() { }
        public DeleteTemplateException(string message) : base(message) { }
        public DeleteTemplateException(string message, System.Exception inner) : base(message, inner) { }
    }
}