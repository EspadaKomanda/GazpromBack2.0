namespace KafkaTestLib.KafkaException;

public class MyKafkaException : Exception
{
    public MyKafkaException()
    {
        
    }

    public MyKafkaException(string message)
        : base(message)
    {
        
    }

    public MyKafkaException(string message, Exception innerException)
        : base(message, innerException)
    {
        
    }
}