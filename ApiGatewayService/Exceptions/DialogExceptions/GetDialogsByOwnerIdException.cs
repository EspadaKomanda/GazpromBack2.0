namespace ApiGatewayService.Exceptions.DialogExceptions
{
    [System.Serializable]
    public class GetDialogsByOwnerIdException : System.Exception
    {
        public GetDialogsByOwnerIdException() { }
        public GetDialogsByOwnerIdException(string message) : base(message) { }
        public GetDialogsByOwnerIdException(string message, System.Exception inner) : base(message, inner) { }
    }
}