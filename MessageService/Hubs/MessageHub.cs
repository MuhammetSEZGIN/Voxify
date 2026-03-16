using System;
using MessageService.DTOs;
using MessageService.Interfaces.Services;
using MessageService.Models;
using MessageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

namespace MessageService.Hubs;

[Authorize]
public class MessageHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly ILogger<MessageHub> _logger;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MessageHub(IMessageService messageService,
        ILogger<MessageHub> logger,
        IServiceScopeFactory serviceScopeFactory,
        IBackgroundTaskQueue backgroundTaskQueue
      )
    {
        _backgroundTaskQueue = backgroundTaskQueue;
        _serviceScopeFactory = serviceScopeFactory;
        _messageService = messageService;
        _logger = logger;
    }
    public async Task SendMessage(string channelId, string clanId, string senderId, string userName, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            _logger.LogWarning("Empty message attempted to be send by {UserName}", userName);
            return;
        }

        try
        {
            var objectId = ObjectId.GenerateNewId();    
            var messageDto = new MessageDto
            {
                Id = objectId.ToString(),
                ClanId = clanId,
                UserName = userName,
                ChannelId = channelId,
                SenderId = senderId,
                Text = message,
                CreatedAt = DateTime.UtcNow
            };

            await Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", messageDto);

            var newMessage = new Message
            {
                Id = objectId,
                ClanId = clanId,
                ChannelId = channelId,
                SenderId = senderId,
                Text = message,
                CreatedAt = DateTime.UtcNow
            };

            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                    await messageService.CreateMessage(newMessage);
                    _logger.LogInformation("Message {MessageId} succesfully saved to database", newMessage.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error saving message to database");
                }
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in SendMessage for channel {ChannelId} by {UserName}", channelId, userName);
            throw;
        }
    }

    public async Task UpdateMessage(ObjectId messageId, string newContent)
    {
        if (string.IsNullOrEmpty(newContent))
        {
            _logger.LogWarning("Empty content in UpdateMessage for {MessageId}", messageId);
            return;
        }
        await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async token =>
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var result = await messageService.UpdateMessage(messageId, newContent);
                if (result == null)
                {
                    _logger.LogWarning("Message {MessageId} not found for update", messageId);
                    return;
                }

                var messageDto = new MessageDto
                {
                    Id = result.Data.Id.ToString(),
                    ClanId = result.Data.ClanId,
                    ChannelId = result.Data.ChannelId,
                    SenderId = result.Data.SenderId,
                    Text = result.Data.Text,
                    CreatedAt = result.Data.CreatedAt
                };
                await Clients.Group(result.Data.ChannelId.ToString()).SendAsync("MessageUpdated", messageDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating message {MessageId}", messageId);
                await Clients.Caller.SendAsync("MessageUpdateFailed", messageId);
            }
        });
    }

    public async Task JoinChannel(Guid channelId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, channelId.ToString());
    }

    public async Task LeaveChannel(Guid channelId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId.ToString());
    }
}
