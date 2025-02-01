using System;

namespace IdentityService.Messaging.RabbitMQ;

public class RabbitMqOptions
{

        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string ExchangeName { get; set; } = "IdentityExchange";
        public string QueueName { get; set; } = "UserUpdatedQueue";
        public string RoutingKey { get; set; } = "user.updated";
       
}
