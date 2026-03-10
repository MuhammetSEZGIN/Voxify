using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace MessageService.Hubs;

public class VoicePresenceHub : Hub
{
    // clanId → voiceChannelId → participants
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, List<UserInfo>>> _presence
        = new();

    // connectionId → (ClanId, ChannelId, UserId) — cleanup on disconnect
    private static readonly ConcurrentDictionary<string, (string ClanId, string ChannelId, string UserId)> _connections
        = new();

    private readonly ILogger<VoicePresenceHub> _logger;

    public VoicePresenceHub(ILogger<VoicePresenceHub> logger)
    {
        _logger = logger;
    }

    // Client → Server: user joined a LiveKit room
    public async Task JoinVoiceChannel(string clanId, string voiceChannelId, string userId, string userName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"clan_{clanId}");

        var channels = _presence.GetOrAdd(clanId, _ => new ConcurrentDictionary<string, List<UserInfo>>());
        var participants = channels.GetOrAdd(voiceChannelId, _ => new List<UserInfo>());

        lock (participants)
        {
            participants.RemoveAll(u => u.UserId == userId);
            participants.Add(new UserInfo(userId, userName));
        }

        _connections[Context.ConnectionId] = (clanId, voiceChannelId, userId);

        _logger.LogInformation("User {UserId} joined voice channel {ChannelId} in clan {ClanId}", userId, voiceChannelId, clanId);

        await Clients.Group($"clan_{clanId}").SendAsync("UserJoinedVoice", new
        {
            clanId,
            voiceChannelId,
            userId,
            userName
        });
    }

    // Client → Server: user left a LiveKit room
    public async Task LeaveVoiceChannel(string clanId, string voiceChannelId, string userId)
    {
        RemoveParticipant(clanId, voiceChannelId, userId);
        _connections.TryRemove(Context.ConnectionId, out _);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"clan_{clanId}");

        _logger.LogInformation("User {UserId} left voice channel {ChannelId} in clan {ClanId}", userId, voiceChannelId, clanId);

        await Clients.Group($"clan_{clanId}").SendAsync("UserLeftVoice", new
        {
            clanId,
            voiceChannelId,
            userId
        });
    }

    // Client → Server: request current snapshot (e.g. on page refresh)
    public async Task GetVoiceChannelParticipants(string clanId)
    {
        var result = new List<object>();

        if (_presence.TryGetValue(clanId, out var channels))
        {
            foreach (var (channelId, users) in channels)
            {
                List<UserInfo> snapshot;
                lock (users)
                {
                    snapshot = new List<UserInfo>(users);
                }

                foreach (var user in snapshot)
                {
                    result.Add(new { voiceChannelId = channelId, userId = user.UserId, userName = user.UserName });
                }
            }
        }

        await Clients.Caller.SendAsync("VoiceChannelParticipants", new
        {
            clanId,
            participants = result
        });
    }

    // Auto-cleanup when the SignalR connection drops
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (_connections.TryRemove(Context.ConnectionId, out var info))
        {
            RemoveParticipant(info.ClanId, info.ChannelId, info.UserId);

            _logger.LogInformation(
                "Connection dropped — removing user {UserId} from voice channel {ChannelId} in clan {ClanId}",
                info.UserId, info.ChannelId, info.ClanId);

            await Clients.Group($"clan_{info.ClanId}").SendAsync("UserLeftVoice", new
            {
                clanId = info.ClanId,
                voiceChannelId = info.ChannelId,
                userId = info.UserId
            });
        }

        await base.OnDisconnectedAsync(exception);
    }

    private static void RemoveParticipant(string clanId, string voiceChannelId, string userId)
    {
        if (!_presence.TryGetValue(clanId, out var channels) ||
            !channels.TryGetValue(voiceChannelId, out var participants))
            return;

        lock (participants)
        {
            participants.RemoveAll(u => u.UserId == userId);
        }

        // Prune empty entries to avoid unbounded growth
        if (participants.Count == 0)
            channels.TryRemove(voiceChannelId, out _);

        if (channels.IsEmpty)
            _presence.TryRemove(clanId, out _);
    }

    private record UserInfo(string UserId, string UserName);
}
