using System;
using System.Security.Authentication;
using IdentityService.Messaging.RabbitMQ;
using MassTransit;

namespace IdentityService.Messaging;

 public static class MassTransitProducerConfiguration
    {
        public static IServiceCollection AddRabbitMQProducer(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqOptions = new RabbitMqOptions();
            configuration.GetSection("RabbitMQ").Bind(rabbitMqOptions);
            
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                   cfg.Host(rabbitMqOptions.Host, (ushort)rabbitMqOptions.Port, rabbitMqOptions.VirtualHost, h => 
                    {
                        h.Username(rabbitMqOptions.UserName);
                        h.Password(rabbitMqOptions.Password);
                        // Use SSL if connecting via amqps (CloudAMQP default port 5671)
                        if (rabbitMqOptions.Port == 5671)
                        {
                            h.UseSsl(s =>
                            {
                                s.Protocol = SslProtocols.Tls12;
                            });
                        }
                    });
                });
            });
            
            return services;
        }
    }
