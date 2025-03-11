using ClanService.Data;
using ClanService.Models;
using ClanService.Interfaces;
using Microsoft.EntityFrameworkCore;
using ClanService.RabbitMq;
using ClanService.DTOs;

namespace ClanService.Services
{
    public class ChannelService : IChannelService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChannelService> _logger;
        private readonly ClanServicePublisher _publisher;
        public ChannelService(ApplicationDbContext context, ILogger<ChannelService> logger, ClanServicePublisher publisher)
        {
            _publisher= publisher;
            _logger = logger;
            _context = context;
        }

        public async Task<(Channel, string)> CreateChannelAsync(Channel channel)
        {
            try
            {
                var clan = await _context.Clans.FindAsync(channel.ClanId);
                if (clan == null)
                    return (null, "Clan not found");

                channel.ChannelId = Guid.NewGuid();
                await _context.Channels.AddAsync(channel);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Channel {ChannelId} created successfully for clan {ClanId}", channel.ChannelId, channel.ClanId);
                return (channel, "Channel created successfully");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating channel for clan {ClanId}", channel.ClanId);
                return (null, "Error while creating channel");
            }
        }

        public async Task<Channel> GetChannelByIdAsync(Guid channelId)
        {
            return await _context.Channels.AsNoTracking()
            .FirstOrDefaultAsync(c => c.ChannelId == channelId);

        }

        public async Task<List<Channel>> GetChannelsByClanIdAsync(Guid clanId)
        {
            return await _context.Channels.AsNoTracking()
                .Where(c => c.ClanId == clanId)
                .ToListAsync();
        }

        public async Task<Channel> UpdateChannelAsync(Channel channel)
        {
            try
            {
                _context.Channels.Update(channel);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Channel {ChannelId} updated successfully", channel.ChannelId);
                return channel;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating channel {ChannelId}", channel.ChannelId);
                return null;
            }

        }

        public async Task<bool> DeleteChannelAsync(Guid channelId)
        {
            try
            {
                var existing = await _context.Channels.FindAsync(channelId);
                if (existing == null) return false;
                _context.Channels.Remove(existing);
                await _context.SaveChangesAsync();
                await _publisher.PublishDeleteChannelMessageAsync(new ChannelDeletedMessage{
                    ChannelId = channelId
                });
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting channel {ChannelId}", channelId);
                return false;
            }
        }

    }
}
