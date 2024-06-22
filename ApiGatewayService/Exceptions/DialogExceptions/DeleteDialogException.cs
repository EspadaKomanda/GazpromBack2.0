namespace ApiGatewayService.Exceptions.DialogExceptions
{
    [System.Serializable]
    public class DeleteDialogException : System.Exception
    {
        public DeleteDialogException() { }
        public DeleteDialogException(string message) : base(message) { }
        public DeleteDialogException(string message, System.Exception inner) : base(message, inner) { }
    }
}