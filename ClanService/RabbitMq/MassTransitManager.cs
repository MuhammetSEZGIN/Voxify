using MassTransit;
namespace ClanService.RabbitMq;

public static class MassTransitManager
{
    public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = new RabbitMQOptions();
        configuration.GetSection("RabbitMQ").Bind(rabbitMqOptions);
        services.AddMassTransit(x =>
         {
             x.AddConsumer<IdentityConsumer, SubmitIdentityConsumeDefinition>();


             x.UsingRabbitMq((context, cfg) =>
             {
                 cfg.Host(rabbitMqOptions.HostName, "/", h =>
                 {
                     h.Username(rabbitMqOptions.UserName);
                     h.Password(rabbitMqOptions.Password);
                 });

                 cfg.ConfigureEndpoints(context);
             });
         });

        return services;
    }
}
