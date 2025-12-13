using System;
using MongoDB.Bson;

namespace ClanService.DTOs;

public class ChannelDeletedMessage
{
    public string ChannelId { get; set; }
}
