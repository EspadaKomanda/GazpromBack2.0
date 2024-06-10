[System.Serializable]
public class GeneratePromptException : System.Exception
{
    public GeneratePromptException() { }
    public GeneratePromptException(string message) : base(message) { }
    public GeneratePromptException(string message, System.Exception inner) : base(message, inner) { }
    protected GeneratePromptException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}