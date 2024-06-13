namespace ImageAgregationService.Exceptions.TemplateExceptions
{
    [System.Serializable]
    public class TemplateNotFoundException : System.Exception
    {
        public TemplateNotFoundException() { }
        public TemplateNotFoundException(string message) : base(message) { }
        public TemplateNotFoundException(string message, System.Exception inner) : base(message, inner) { }
    }
}