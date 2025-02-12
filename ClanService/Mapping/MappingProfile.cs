using AutoMapper;
using ClanService.DTOs;
using ClanService.Models;

namespace ClanService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Channel mappings
            CreateMap<ChannelCreateDto, Channel>();
            CreateMap<Channel, ChannelReadDto>();
            CreateMap<ChannelUpdateDto, Channel>();

            // Clan mappings
            CreateMap<ClanCreateDto, Clan>();
            CreateMap<Clan, ClanReadDto>();
            CreateMap<ClanUpdateDto, Clan>();
            CreateMap<Clan, GetAllClanPropertyDto>();   
            CreateMap<GetAllClanPropertyDto, Clan>();   
            // ClanMembership mappings
            CreateMap<ClanMembershipCreateDto, ClanMembership>();
            CreateMap<ClanMembership, ClanMembershipReadDto>();

            // VoiceChannel mappings
            CreateMap<VoiceChannelCreateDto, VoiceChannel>();
            CreateMap<VoiceChannel, VoiceChannelReadDto>();
            CreateMap<VoiceChannelUpdateDto, VoiceChannel>();

        }
    }
}