using System;
using Identity.DTOs;

namespace ClanService.Interfaces;

public interface IRabbitMqService
{
    Task ConsumeUserInformation(UserUpdatedMessage userUpdatedMessage);
}
