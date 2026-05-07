using System;
using MongoDB.Bson;

namespace MessageService.DTOs;

public class MessageDto
{
    public string Id { get; set; }
    public string ClanId { get; set; }
    public string ChannelId { get; set; }
    public string UserName { get; set; }
    public string SenderId { get; set; }
    public string AvatarUrl { get; set; }   
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }

}
