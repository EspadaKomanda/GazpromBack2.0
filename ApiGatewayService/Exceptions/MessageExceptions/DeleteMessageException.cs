namespace ApiGatewayService.Exceptions.MessageExceptions
{
   [System.Serializable]
   public class DeleteMessageException : System.Exception
   {
    public DeleteMessageException() { }
    public DeleteMessageException(string message) : base(message) { }
    public DeleteMessageException(string message, System.Exception inner) : base(message, inner) { }
   }
}