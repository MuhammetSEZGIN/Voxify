using System;

namespace MessageService.Models;

public class ClanMemberShip
{
    public int Id { get; set; }
    public int ClanId { get; set; }
    public Clan Clan { get; set; }

    public string UserId { get; set; }=null;
}
