using System;
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
                    cfg.Host(rabbitMqOptions.HostName, "/", h =>
                    {
                        h.Username(rabbitMqOptions.UserName);
                        h.Password(rabbitMqOptions.Password);
                    });
                   
                });
            });
            
            return services;
        }
    }
