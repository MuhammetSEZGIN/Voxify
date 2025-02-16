using System;
using MessageService.Data;
using MessageService.Interfaces;
using MessageService.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Services;

public class MessageService : IMessageService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<MessageService> _logger;
    public MessageService(ApplicationDbContext db, ILogger<MessageService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Message> CreateMessage(Message message)
    {
        try
        {
            _db.Messages.Add(message);
            message.CreatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }catch(Exception e)
        {
            _logger.LogError(e, "Error saving message to database");
        }
        return message;
    }

    public async Task DeleteMessage(Guid messageId)
    {
        var message = await _db.Messages.FindAsync(messageId);
        if (message == null)
            return;

        _db.Messages.Remove(message);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> GetMessagesInChannelAsync(Guid channelId, int limit)
    {
        var messages = await _db.Messages
        .Where(x => x.ChannelId == channelId)
        .Include(x => x.User).Where(x => x.User.Id == x.SenderId)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();
        _logger.LogInformation("Found {Count} messages for channelId {ChannelId}", messages.Count, channelId);
        return messages.Take(limit);
    }

    public async Task<Message> UpdateMessage(Guid messageId, string newContent)
    {
        var message = await _db.Messages.FindAsync(messageId);
        if (message == null)
            return null;

        message.Text = newContent;
        await _db.SaveChangesAsync();
        return message;
    }

}
