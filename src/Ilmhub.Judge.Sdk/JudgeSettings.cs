namespace Ilmhub.Judge.Sdk;

public class JudgeMessagingSettings {
    public string Driver { get; set; } = "RabbitMQ";
    public RabbitMQSettings RabbitMQ { get; set; }

    public class RabbitMQSettings {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}