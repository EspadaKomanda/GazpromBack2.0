﻿namespace KafkaTestLib.KafkaException;

public class CreateTopicException : TopicException
{
    public CreateTopicException()
    {
    }

    public CreateTopicException(string message)
        : base(message)
    {
    }

    public CreateTopicException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}