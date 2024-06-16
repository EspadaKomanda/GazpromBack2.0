namespace ApiGatewayService.Exceptions.DialogExceptions
{
    [System.Serializable]
    public class ClearDialogException : System.Exception
    {
        public ClearDialogException() { }
        public ClearDialogException(string message) : base(message) { }
        public ClearDialogException(string message, System.Exception inner) : base(message, inner) { }
        protected ClearDialogException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}