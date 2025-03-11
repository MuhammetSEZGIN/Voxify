using System;
using MessageService.Data;
using MessageService.Interfaces.Services;
using MessageService.Models;
using MessageService.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Repositories;

public class MessageRepository : Repository<Message, Guid>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetMessagesInChannelAsync(Guid channelId, int limit, int page)
    {
        return await _context.Messages
              .AsNoTracking()
              .Where(x => x.ChannelId == channelId)
              .Include(x => x.User)
              .OrderByDescending(x => x.CreatedAt)
              .Skip((page - 1) * limit)
              .Take(limit)
              .ToListAsync();
    }

    public async Task<IEnumerable<Message>> SearchInChannelAsync(Guid channelId, string searchText, int limit, int page)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return await GetMessagesInChannelAsync(channelId, limit, page);
        }

        return await _context.Messages
            .AsNoTracking()
            .Where(x => x.ChannelId == channelId &&
                       EF.Functions.ILike(x.Text, $"%{searchText}%"))
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> DeleteMessagesOfChannelByChannelId(Guid channelId){
        var messages = await _context.Messages.Where(x => x.ChannelId == channelId).ToListAsync();
        if(messages.Count == 0){
            return false;
        }
        _context.Messages.RemoveRange(messages);
        await _context.SaveChangesAsync();
        return true;
    }
}
