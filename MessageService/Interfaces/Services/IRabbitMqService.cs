using System;
using ClanService.DTOs;
using Identity.DTOs;

namespace MessageService.Interfaces.Services;

public interface IRabbitMqService
{
    Task ConsumeUserInformation(UserUpdatedMessage message);
    Task ConsumeChannelInformation(ChannelDeletedMessage message);

}
