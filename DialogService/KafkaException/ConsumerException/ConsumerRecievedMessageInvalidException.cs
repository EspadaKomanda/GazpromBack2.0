namespace KafkaTestLib.KafkaException.ConsumerException;

public class ConsumerRecievedMessageInvalidException : ConsumerException
{
    public ConsumerRecievedMessageInvalidException()
    {
    }

    public ConsumerRecievedMessageInvalidException(string message)
        : base(message)
    {
    }

    public ConsumerRecievedMessageInvalidException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}