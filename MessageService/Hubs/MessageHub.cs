using System;
using MessageService.DTOs;
using MessageService.Interfaces;
using MessageService.Models;
using Microsoft.AspNetCore.SignalR;

namespace MessageService.Hubs;

public class MessageHub: Hub
{
    private readonly IMessageService _messageService;
    private readonly ILogger<MessageHub> _logger;

    public MessageHub(IMessageService messageService, ILogger<MessageHub> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }
    public async Task SendMessage(Guid channelId, string senderId, string userName, string message)   
    {
      
        var messageDto = new MessageDto
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            ChannelId = channelId,
            SenderId = senderId,
            Text = message,
            CreatedAt = DateTime.UtcNow
        };

        await Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", messageDto);
          var newMessage = new Message
        {
            Id = messageDto.Id,    
            ChannelId = channelId,
            SenderId = senderId,
            Text = message,
            CreatedAt = DateTime.UtcNow 
        };
        _ = Task.Run(async () =>
        {
            try{
            await _messageService.CreateMessage(newMessage);

            }catch(Exception e){
                _logger.LogError(e, "Error saving message to database");
            }
        }); 
    }
 
    public async Task UpdateMessage(Guid messageId, string newContent)
    {
        var updatedMessage = await _messageService.UpdateMessage(messageId, newContent);
        if(updatedMessage == null)
            return;
        var messageDto = new MessageDto
        {
            Id = updatedMessage.Id,
            UserName = updatedMessage.User.UserName,
            ChannelId = updatedMessage.ChannelId,
            SenderId = updatedMessage.SenderId,
            Text = updatedMessage.Text,
            CreatedAt = updatedMessage.CreatedAt
        };
        await Clients.Group(updatedMessage.ChannelId.ToString()).SendAsync("MessageUpdated", messageDto);
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
