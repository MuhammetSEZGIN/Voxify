using System;

namespace MessageService.Models;

public class Clan
{
    public int ClanId { get; set; }
    public string Name { get; set; }

    public List<Channel> Channels { get; set; }
}
