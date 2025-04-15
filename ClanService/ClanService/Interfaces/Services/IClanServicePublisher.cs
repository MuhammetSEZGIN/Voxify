using System;
using ClanService.DTOs;

namespace ClanService.Interfaces.Services;

public interface IClanServicePublisher
{
    Task PublishDeleteChannelMessageAsync(ChannelDeletedMessage message);

}
