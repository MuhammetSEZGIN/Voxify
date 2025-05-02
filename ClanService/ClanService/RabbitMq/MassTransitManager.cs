using MassTransit;
using System.Security.Authentication;

namespace ClanService.RabbitMq;

public static class MassTransitManager
{
    public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = new RabbitMQOptions(); // Doğru sınıf adını kullanın
        configuration.GetSection("RabbitMQ").Bind(rabbitMqOptions);
        services.AddMassTransit(x =>
         {
             // Consumer'ı ve tanımını ekleyin
             x.AddConsumer<IdentityConsumer, SubmitIdentityConsumeDefinition>();

             x.UsingRabbitMq((context, cfg) =>
             {
                 // Güncellenmiş seçenekleri ve SSL'i kullanın
                 cfg.Host(rabbitMqOptions.HostName, (ushort)rabbitMqOptions.Port, rabbitMqOptions.VirtualHost, h =>
                 {
                     h.Username(rabbitMqOptions.UserName);
                     h.Password(rabbitMqOptions.Password);
                     if (rabbitMqOptions.Port == 5671) // Port 5671 ise SSL kullan
                     {
                         h.UseSsl(s =>
                         {
                             s.Protocol = SslProtocols.Tls12;
                         });
                     }
                 });
                    cfg.UseMessageRetry(r=>
                    {
                        r.Interval(5, TimeSpan.FromSeconds(10));
                    });
                 // Endpoint'leri otomatik yapılandır
                 cfg.ConfigureEndpoints(context);
             });
         });
        return services;
    }
}