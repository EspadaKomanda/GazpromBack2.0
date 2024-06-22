namespace ApiGatewayService.Exceptions.DialogExceptions
{
    [System.Serializable]
    public class ClearDialogException : System.Exception
    {
        public ClearDialogException() { }
        public ClearDialogException(string message) : base(message) { }
        public ClearDialogException(string message, System.Exception inner) : base(message, inner) { }
    }
}