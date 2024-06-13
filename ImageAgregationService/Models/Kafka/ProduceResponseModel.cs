using Confluent.Kafka;

namespace KafkaTestLib.Models;

public class ProduceResponseModel 
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = null!;
    public Message<string, TestResponse> Message { get; set; } = null!;
}
