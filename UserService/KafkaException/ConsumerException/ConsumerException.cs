namespace KafkaTestLib.KafkaException.ConsumerException;

public class ConsumerException : MyKafkaException
{
    public ConsumerException()
    {
    }

    public ConsumerException(string message)
        : base(message)
    {
    }

    public ConsumerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}