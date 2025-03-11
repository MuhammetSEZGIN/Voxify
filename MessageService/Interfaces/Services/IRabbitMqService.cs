using System;
using ClanService.DTOs;
using Identity.DTOs;

namespace MessageService.Interfaces;

public interface IRabbitMqService
{
    Task ConsumeUserInformation(UserUpdatedMessage message);
    Task ConsumeChannelInformation(ChannelDeletedMessage message);

}
