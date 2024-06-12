namespace ImageAgregationService.Exceptions.MarkExceptions
{
    [System.Serializable]
    public class AddMarkException : System.Exception
    {
        public AddMarkException() { }
        public AddMarkException(string message) : base(message) { }
        public AddMarkException(string message, System.Exception inner) : base(message, inner) { }
        protected AddMarkException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}