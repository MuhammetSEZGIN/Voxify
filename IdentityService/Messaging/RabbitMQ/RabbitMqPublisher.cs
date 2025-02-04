using IdentityService.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace IdentityService.Messaging.RabbitMQ;

public class RabbitMqPublisher : IMessagePublisher

{
    private readonly RabbitMqManager _rabbitMqManager;
    private IChannel _channel;
    public RabbitMqPublisher(RabbitMqManager rabbitMqManager)
    {
        _rabbitMqManager = rabbitMqManager;
    }
    public async Task PublishAsync<T>(T message)
    {
        _channel = await _rabbitMqManager.CreateChannel();
        var exchangeName = _rabbitMqManager.RabbitMqOptions.ExchangeName;
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        var routingKey = _rabbitMqManager.RabbitMqOptions.RoutingKey;
        var props = new BasicProperties();
        props.Persistent = false; // we dont want any performance issues

       
            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body.AsMemory());
    }
}

