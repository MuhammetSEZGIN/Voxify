using IdentityService.Messaging.RabbitMQ;
using RabbitMQ.Client;


namespace IdentityService.Messaging;
using System;
using Microsoft.Extensions.Options;

public class RabbitMqManager 
{ 
    private readonly RabbitMqOptions _rabbitMqOptions;
    public RabbitMqManager(IOptions<RabbitMqOptions> rabbitMqOptions){
        _rabbitMqOptions = rabbitMqOptions.Value;
       
    }
    public RabbitMqOptions RabbitMqOptions => _rabbitMqOptions;
    public async Task<IChannel> CreateChannel(){
        var factory = new ConnectionFactory()
        { 
            HostName = _rabbitMqOptions.HostName,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password
        };
        var _connection = await factory.CreateConnectionAsync();
        var _channel = await _connection.CreateChannelAsync();
        return _channel;
    }

}
