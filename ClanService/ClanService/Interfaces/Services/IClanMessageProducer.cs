using System;
using Shared.Contracts;

namespace ClanService.Interfaces.Services;

public interface IClanMessageProducer
{
    Task PublishChannelDeletedMessageAsync(
      string channelId,
      string clanId,
      ChannelType channelType
  );
}