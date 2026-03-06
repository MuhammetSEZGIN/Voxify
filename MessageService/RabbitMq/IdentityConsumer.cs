using System;
using MassTransit;
using Identity.DTOs;
using MessageService.Interfaces;
using System.Threading.Channels;
using MessageService.Interfaces.Services;
namespace MessageService.RabbitMq;

public class IdentityConsumer : IConsumer<UserUpdatedMessage>
{
    private readonly ILogger<IdentityConsumer> _logger;
    
    private readonly IRabbitMqService _rabbitMqService;
    public IdentityConsumer(ILogger<IdentityConsumer> logger , IRabbitMqService rabbitMqService)
    {
        _logger = logger;
        _rabbitMqService = rabbitMqService;
    }
    public async Task Consume(ConsumeContext<UserUpdatedMessage> context)
    {
        _logger.LogInformation($"Received message: {context.Message.userName} \n {context.Message.AvatarUrl}");
        await _rabbitMqService.ConsumeUserInformation(context.Message); 
    }
   

}
