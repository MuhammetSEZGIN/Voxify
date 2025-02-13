using System;
using MassTransit;

namespace MessageService.RabbitMq;

public class SubmitIdentityConsumeDefinition : ConsumerDefinition<IdentityConsumer>
{

    public SubmitIdentityConsumeDefinition()
    {
        EndpointName= "UserUpdatedQueue";
        ConcurrentMessageLimit = 1;

    }
     protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<IdentityConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

            // Outbox kullanarak duplicate event yayınlarının önüne geçiyoruz
            // ama consumer tarafından bir publish işlemi yapılmadığı için kullanmaya gerek yok
            // endpointConfigurator.UseInMemoryOutbox(context);
        }
}

