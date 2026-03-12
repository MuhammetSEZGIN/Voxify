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
    Member,
    Admin,
    Owner
}

public record class ChannelDeletedMessage
{
    public string ?ChannelId { get; set; }
    public string ?ClanId { get; set; }
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

