namespace Shared.Contracts;

public enum ChannelType
{
    VoiceChannel,
    TextChannel
}
public enum MessageType
{
    ClanDeleted,
    TextChannelDeleted,
    VoiceChannelDeleted,
}
public enum ClanRole
{
    MEMBER,
    ADMIN,
    OWNER
}
public enum ClanRoleEventType
{
    ASSIGN_ROLE,
    REMOVE_ROLE,
    REMOVE_ALL_ROLES
}

public record ClanRoleEventDto
{
    public string? UserId { get; init; }
    public string? ClanId { get; init; }
    public string? Role { get; init; }
    public string? EventType { get; init; }
}
public record class ChannelDeletedMessage
{
    public string? ChannelId { get; set; }
    public string? ClanId { get; set; }
    public ChannelType ChannelType { get; set; }
}

public record UserUpdatedMessage
{
    public string? userId { get; init; }
    public string? userName { get; init; }
    public string? AvatarUrl { get; init; }

}
public record ClanDeletedMessage
{
    public string? ClanId { get; init; }
}

