namespace ApiGatewayService.Exceptions.DialogExceptions
{
    [System.Serializable]
    public class GetDialogMessagesException : System.Exception
    {
        public GetDialogMessagesException() { }
        public GetDialogMessagesException(string message) : base(message) { }
        public GetDialogMessagesException(string message, System.Exception inner) : base(message, inner) { }
    }
}