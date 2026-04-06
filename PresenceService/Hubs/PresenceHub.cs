using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PresenceService.Interfaces;

namespace PresenceService.Hubs;

[Authorize(AuthenticationSchemes = "Bearer")]
public class PresenceHub : Hub
{
    private readonly IPresenceRepository _repository;
    private readonly ILogger<PresenceHub> _logger;

    public PresenceHub(IPresenceRepository repository, ILogger<PresenceHub> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // ── Lifecycle ──────────────────────────────────────────────────────────────

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await _repository.SetUserOnline(userId, Context.ConnectionId);
            _logger.LogInformation("User {UserId} connected", userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await _repository.SetUserOffline(userId);

            // Clean up voice channel if the user was in one
            var voiceInfo = await _repository.LeaveVoiceChannel(Context.ConnectionId);
            if (voiceInfo.HasValue)
            {
                var (clanId, channelId, uid) = voiceInfo.Value;
                _logger.LogInformation(
                    "Connection dropped — removing user {UserId} from voice channel {ChannelId} in clan {ClanId}",
                    uid, channelId, clanId);

                await Clients.Group($"clan_{clanId}").SendAsync("UserLeftVoice", new
                {
                    clanId,
                    voiceChannelId = channelId,
                    userId = uid
                });
            }

            // Notify subscribed clans that this user is offline
            var clanIds = await _repository.GetConnectionClans(Context.ConnectionId);
            foreach (var clanId in clanIds)
            {
                await Clients.Group($"clan_{clanId}").SendAsync("UserOffline", userId);
            }

            await _repository.RemoveConnectionClans(Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    // ── Online presence ────────────────────────────────────────────────────────

    /// <summary>
    /// Client calls this after connecting to subscribe to clan presence events.
    /// The server adds the connection to each clan group and notifies members.
    /// </summary>
    public async Task SubscribeToClans(List<string> clanIds)
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId) || clanIds == null || clanIds.Count == 0) return;

        foreach (var clanId in clanIds)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"clan_{clanId}");

        await _repository.SetConnectionClans(Context.ConnectionId, clanIds);

        // Notify each clan that this user is now online
        foreach (var clanId in clanIds)
            await Clients.Group($"clan_{clanId}").SendAsync("UserOnline", userId);
    }

    /// <summary>
    /// Returns which of the given userIds are currently online.
    /// </summary>
    public async Task GetOnlineUsers(List<string> userIds)
    {
        var onlineUsers = new List<string>();
        foreach (var uid in userIds)
        {
            if (await _repository.IsUserOnline(uid))
                onlineUsers.Add(uid);
        }
        await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
    }

    // ── Voice channel presence ─────────────────────────────────────────────────

    /// <summary>
    /// Client calls this when joining a LiveKit voice room.
    /// </summary>
    public async Task JoinVoiceChannel(string clanId, string voiceChannelId, string userName)
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId)) return;

        // Ensure the connection is in the clan group (may already be from SubscribeToClans)
        await Groups.AddToGroupAsync(Context.ConnectionId, $"clan_{clanId}");

        await _repository.JoinVoiceChannel(Context.ConnectionId, userId, userName, clanId, voiceChannelId);

        _logger.LogInformation("User {UserId} joined voice channel {ChannelId} in clan {ClanId}", userId, voiceChannelId, clanId);

        await Clients.Group($"clan_{clanId}").SendAsync("UserJoinedVoice", new
        {
            clanId,
            voiceChannelId,
            userId,
            userName
        });
    }

    /// <summary>
    /// Client calls this when leaving a LiveKit voice room.
    /// </summary>
    public async Task LeaveVoiceChannel()
    {
        var info = await _repository.LeaveVoiceChannel(Context.ConnectionId);
        if (!info.HasValue) return;

        var (clanId, channelId, userId) = info.Value;

        _logger.LogInformation("User {UserId} left voice channel {ChannelId} in clan {ClanId}", userId, channelId, clanId);

        await Clients.Group($"clan_{clanId}").SendAsync("UserLeftVoice", new
        {
            clanId,
            voiceChannelId = channelId,
            userId
        });
    }

    /// <summary>
    /// Returns the current voice channel snapshot for a clan (e.g. on page refresh).
    /// </summary>
    public async Task GetVoiceChannelParticipants(string clanId)
    {
        var channels = await _repository.GetVoiceChannelParticipants(clanId);

        var participants = channels
            .SelectMany(kv => kv.Value.Select(u => new
            {
                voiceChannelId = kv.Key,
                userId = u.UserId,
                userName = u.UserName
            }))
            .ToList();

        await Clients.Caller.SendAsync("VoiceChannelParticipants", new
        {
            clanId,
            participants
        });
    }
}

