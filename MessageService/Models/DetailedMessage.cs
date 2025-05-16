using System;

namespace MessageService.Models;

public class DetailedMessage : Message
{
    public User Users { get; set; }
}
