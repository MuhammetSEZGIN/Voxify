using ClanService.RabbitMq;
using MassTransit;

namespace ClanService.Messaging;

 public static class MassTransitProducerConfiguration
    {
        public static IServiceCollection AddRabbitMQProducer(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqOptions = new RabbitMQOptions();
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
