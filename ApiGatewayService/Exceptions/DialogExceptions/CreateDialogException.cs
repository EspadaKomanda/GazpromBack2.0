namespace ApiGatewayService.Exceptions.DialogExceptions
{
    [System.Serializable]
    public class CreateDialogException : System.Exception
    {
        public CreateDialogException() { }
        public CreateDialogException(string message) : base(message) { }
        public CreateDialogException(string message, System.Exception inner) : base(message, inner) { }
    }
}