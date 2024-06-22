namespace ApiGatewayService.Exceptions.User
{
    [System.Serializable]
    public class ChangePasswordException : System.Exception
    {
        public ChangePasswordException() { }
        public ChangePasswordException(string message) : base(message) { }
        public ChangePasswordException(string message, System.Exception inner) : base(message, inner) { }
    }
}