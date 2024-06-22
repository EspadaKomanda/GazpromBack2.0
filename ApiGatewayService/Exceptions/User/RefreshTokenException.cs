namespace ApiGatewayService.Exceptions.User
{
    [System.Serializable]
    public class RefreshTokenException : System.Exception
    {
        public RefreshTokenException() { }
        public RefreshTokenException(string message) : base(message) { }
        public RefreshTokenException(string message, System.Exception inner) : base(message, inner) { }
    }
}