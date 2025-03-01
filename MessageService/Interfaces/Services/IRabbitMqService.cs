using System;
using Identity.DTOs;

namespace MessageService.Interfaces;

public interface IRabbitMqService
{
    Task ConsumeUserInformation(UserUpdatedMessage message);

}
