[System.Serializable]
public class GetTemplateException : System.Exception
{
    public GetTemplateException() { }
    public GetTemplateException(string message) : base(message) { }
    public GetTemplateException(string message, System.Exception inner) : base(message, inner) { }
}