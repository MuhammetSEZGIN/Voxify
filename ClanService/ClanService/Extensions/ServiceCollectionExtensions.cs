using ClanService.Interfaces;
using ClanService.Interfaces.Repositories;
using ClanService.Interfaces.Services;
using ClanService.Mapping;
using ClanService.RabbitMq;
using ClanService.Repositories;
using ClanService.Services;

namespace ClanService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<IChannelService, ChannelService>();
        services.AddScoped<IClanService, ClanService.Services.ClanService>();
        services.AddScoped<IRabbitMqService, RabbitMqService>();
        services.AddScoped<IClanMembershipService, ClanMembershipService>();
        services.AddScoped<IVoiceChannelService, VoiceChannelService>();
        services.AddScoped<IRoleService, RoleService>();

        services.AddScoped<IChannelRepository, ChannelRepository>();
        services.AddScoped<IClanRepository, ClanRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClanMembershipRepository, ClanMembershipRepository>();
        services.AddScoped<IClanInvitation, ClanInvitationRepository>();
        services.AddScoped<IVoiceChannelRepository, VoiceChannelRepository>();

        services.AddScoped<IClanMessageProducer, ClanMessageProducer>();
        return services;
    }
}
