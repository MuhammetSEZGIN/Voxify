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

           CreateMap<ClanMembership, UserMembershipDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<Clan, GetAllClanPropertyDto>()
                .ForMember(dest => dest.Channels, opt => opt.MapFrom(src => src.Channels))  
                .ForMember(dest => dest.VoiceChannels, opt => opt.MapFrom(src => src.VoiceChannels))
                .ForMember(dest => dest.ClanMemberships, opt => opt.MapFrom(src => src.ClanMemberShips));
                
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