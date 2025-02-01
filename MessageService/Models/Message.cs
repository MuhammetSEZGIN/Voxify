using System;

namespace MessageService.Models;

public class Message
{
    public int Id { get; set; }
    public string SenderId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

    public int ChannelId { get; set; }
    public Channel Channel { get; set; }
    public string RecipientId { get; set; }

}
