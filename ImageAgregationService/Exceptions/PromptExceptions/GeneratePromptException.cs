[System.Serializable]
public class GeneratePromptException : System.Exception
{
    public GeneratePromptException() { }
    public GeneratePromptException(string message) : base(message) { }
    public GeneratePromptException(string message, System.Exception inner) : base(message, inner) { }
}