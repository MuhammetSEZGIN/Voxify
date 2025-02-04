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

    public async Task<IEnumerable<Message>> GetMessagesInChannelAsync(int channelId, int limit){
       return  await _db.Messages
                .Where(m => m.ChannelId == channelId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync();
 
    }


}
