using System;
using MessageService.Data;
using MessageService.Interfaces;
using MessageService.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Services;

public class MessageService : IMessageService
{
    private readonly ApplicationDbContext _db;

    public MessageService(ApplicationDbContext db){
        _db = db;
    }

    public async Task<Message> CreateMessage(Message message)
    {
        _db.Messages.Add(message);
        message.CreatedAt = DateTime.UtcNow;
        _db.Messages.Add(message);
        await _db.SaveChangesAsync();
        return message;
    }

    public async Task DeleteMessage(Guid messageId)
    {
        var message = await _db.Messages.FindAsync(messageId); 
        if(message == null)
            return;
        
        _db.Messages.Remove(message);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> GetMessagesInChannelAsync(Guid channelId, int limit){
       return  await _db.Messages
                .Include(m => m.User)
                .Where(m => m.ChannelId == channelId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync();
 
    }

    public async Task<Message> UpdateMessage(Guid messageId, string newContent)
    {
       var message = await _db.Messages.FindAsync(messageId);  
         if(message == null)
                return null;
          
          message.Text = newContent;
          await _db.SaveChangesAsync();
          return message;
    }

}
