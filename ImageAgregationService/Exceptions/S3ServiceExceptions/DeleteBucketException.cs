namespace ImageAgregationService.Exceptions.S3ServiceExceptions
{
   [System.Serializable]
   public class DeleteBucketException : System.Exception
   {
    public DeleteBucketException() { }
    public DeleteBucketException(string message) : base(message) { }
    public DeleteBucketException(string message, System.Exception inner) : base(message, inner) { }
   }
}