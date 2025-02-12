using System;

namespace MessageService.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ChannelId { get; set; }
    public string UserName { get; set; }
    public string SenderId { get; set; }
    public string AvatarUrl { get; set; }   
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }

}
