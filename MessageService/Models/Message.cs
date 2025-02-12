using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageService.Models;

public class Message
{
    [Key]
    public Guid Id { get; set; }
    public string SenderId { get; set; }
    [ForeignKey("SenderId")]
    public User User { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
    public Guid ChannelId { get; set; }
    public string RecipientId { get; set; }

}
