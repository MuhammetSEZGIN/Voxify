using Shared.Contracts;
namespace MessageService.Interfaces.Services;

public interface IRabbitMqService
{
    Task ConsumeUserInformation(UserUpdatedMessage message);
    Task ConsumeChannelInformation(ChannelDeletedMessage message);

}
