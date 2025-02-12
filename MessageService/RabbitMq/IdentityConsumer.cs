using System;
using MassTransit;
using Identity.DTOs;
using MessageService.Interfaces;
using System.Threading.Channels;
namespace MessageService.RabbitMq;

public class IdentityConsumer : IConsumer<UserUpdatedMessage>
{
    ILogger<IdentityConsumer> _logger;
    
    IRabbitMqService    _rabbitMqService;
    public IdentityConsumer(ILogger<IdentityConsumer> logger , IRabbitMqService rabbitMqService)
    {
        _logger = logger;
        _rabbitMqService = rabbitMqService;
    }
    public async Task Consume(ConsumeContext<UserUpdatedMessage> context)
    {
        _logger.LogInformation($"Received message: {context.Message.userName} \n {context.Message.avatarUrl}");
        await _rabbitMqService.ConsumeUserInformation(context.Message); 
    }
   

}
