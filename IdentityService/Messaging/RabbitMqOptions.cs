using System;

namespace IdentityService.Messaging.RabbitMQ;

public class RabbitMqOptions
{
    public string Host { get; set; }
    public string VirtualHost { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string QueueName { get; set; }
}
