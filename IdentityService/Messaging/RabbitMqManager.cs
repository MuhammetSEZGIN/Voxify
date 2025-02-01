using System.Threading.Tasks;
using IdentityService.Messaging.RabbitMQ;
using RabbitMQ.Client;
namespace IdentityService.Messaging;

public class RabbitMqManager : IDisposable
{ 
    private readonly RabbitMqOptions _rabbitMqOptions;
    public RabbitMqManager(RabbitMqOptions rabbitMqOptions){
        _rabbitMqOptions = rabbitMqOptions;
       
    }
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
     public void Dispose()
    {
        
    }
}
