using System;
using MassTransit;
using Identity.DTOs;
using ClanService.Interfaces;
namespace ClanService.RabbitMq;

public class IdentityConsumer : IConsumer<UserUpdatedMessage>
{
    ILogger<IdentityConsumer> _logger;
    IRabbitMqService   _rabbitMqService;

    public IdentityConsumer(ILogger<IdentityConsumer> logger, IRabbitMqService rabbitMqService)
    {
        _logger = logger;
        _rabbitMqService = rabbitMqService;
    }
    public async Task Consume(ConsumeContext<UserUpdatedMessage> context)
    {
        _logger.LogInformation($"Received message: {context.Message.userName} \n {context.Message.email}");
        await CreateIdentityAsync(context.Message);
    }

    public async Task CreateIdentityAsync(UserUpdatedMessage message)
    {
        await _rabbitMqService.ConsumeUserInformation(message);       
    }
}
