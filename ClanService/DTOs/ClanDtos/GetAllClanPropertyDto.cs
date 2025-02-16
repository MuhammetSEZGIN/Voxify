using System;

namespace ClanService.DTOs;

public class GetAllClanPropertyDto
{

    public Guid ClanId { get; set; }
    public string Name { get; set; }
    public string ImagePath { get; set; }
    public List<ChannelReadDto> Channels { get; set; }
    public List<VoiceChannelReadDto> VoiceChannels { get; set; }
    public List<UserMembershipDto> ClanMemberships { get; set; }

}
