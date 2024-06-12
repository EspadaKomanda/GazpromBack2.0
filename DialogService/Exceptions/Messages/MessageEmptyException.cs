namespace DialogService.Exceptions.Messages;

[Serializable]
public class MessageEmptyException : Exception
{
    public MessageEmptyException() { }
    public MessageEmptyException(string message) : base(message) { }
    public MessageEmptyException(string message, Exception inner) : base(message, inner) { }
}