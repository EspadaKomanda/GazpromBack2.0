namespace KafkaTestLib.KafkaException;

public class ProducerException : MyKafkaException
{
    public ProducerException()
    {
        
    }

    public ProducerException(string message)
        : base(message)
    {
        
    }

    public ProducerException(string message, Exception innerException)
        : base(message, innerException)
    {
        
    }
}