[System.Serializable]
public class GetTemplateException : System.Exception
{
    public GetTemplateException() { }
    public GetTemplateException(string message) : base(message) { }
    public GetTemplateException(string message, System.Exception inner) : base(message, inner) { }
    protected GetTemplateException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}