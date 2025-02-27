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
        }
        catch (Exception e)
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

    public async Task<IEnumerable<Message>> GetMessagesInChannelAsync(Guid channelId, int limit, int page)
    {
        try{
            var messages = await _db.Messages
                .AsNoTracking()
                .Where(x => x.ChannelId == channelId)
                .Include(x => x.User).Where(x => x.User.Id == x.SenderId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
            _logger.LogInformation("Found {Count} messages for channelId {ChannelId}", messages.Count, channelId);
            return messages;
        }catch(Exception e){
            _logger.LogError(e, "Error getting messages for channelId {ChannelId}", channelId);
            return null;
        }
    }

    public async Task<Message> UpdateMessage(Guid messageId, string newContent)
    {
        try
        {
            var message = await _db.Messages.FindAsync(messageId);
            if (message == null)
            {
                _logger.LogError("Message with id {MessageId} not found", messageId);
                return null;
            }

            message.Text = newContent;
            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated message with id {MessageId}", messageId);
            return message;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating message with id {MessageId}", messageId);
            return null;
        }
    }

}
