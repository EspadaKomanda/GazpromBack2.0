[Serializable]
public class DialogNotFoundException : Exception
{
    public DialogNotFoundException() { }
    public DialogNotFoundException(string message) : base(message) { }
    public DialogNotFoundException(string message, Exception inner) : base(message, inner) { }
}