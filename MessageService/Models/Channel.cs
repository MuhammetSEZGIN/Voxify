using System;

namespace MessageService.Models;

public class Channel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ClanId { get; set; }
    public Clan Clan { get; set; }
    public List<Message> Messages { get; set; }

}
