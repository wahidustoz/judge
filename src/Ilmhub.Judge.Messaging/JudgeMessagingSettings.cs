using System.Text.Json.Serialization;

namespace Ilmhub.Judge.Messaging;

public class JudgeMessagingSettings {
    public string Driver { get; set; }
    [JsonPropertyName("RabbitMQ")]
    public RabbitMQSettings RabbitMQ { get; set; }
    public class RabbitMQSettings {
        public string Host { get; set; } = "localhost";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}