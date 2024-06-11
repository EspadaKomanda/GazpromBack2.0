using Newtonsoft.Json;

namespace KafkaTestLib.Models;

public class TestResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
}