[Serializable]
public class MessageEditException : Exception
{
    public MessageEditException() { }
    public MessageEditException(string message) : base(message) { }
    public MessageEditException(string message, Exception inner) : base(message, inner) { }
}