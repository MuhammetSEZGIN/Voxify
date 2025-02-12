using ClanService.Data;
using ClanService.Models;
using ClanService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Services
{
    public class ClanService : IClanService
    {
        private readonly ApplicationDbContext _context;

        public ClanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Clan> CreateClanAsync(Clan clan)
        {
            clan.ClanId = Guid.NewGuid();
            await _context.Clans.AddAsync(clan);
            await _context.SaveChangesAsync();
            return clan;
        }

        public async Task<Clan> GetClanByIdAsync(Guid clanId)
        {
            // Kanallar ve üyelikler dahil çekmek isterseniz Include yapabilirsiniz
            return await _context.Clans
                .Include(c => c.Channels)
                .Include(c => c.VoiceChannels)
                .Include(c => c.ClanMemberShips)
                .FirstOrDefaultAsync(x => x.ClanId == clanId);
        }

        public async Task<List<Clan>> GetAllClansAsync()
        {
            return await _context.Clans.ToListAsync();
        }

        public async Task<Clan> UpdateClanAsync(Clan clan)
        {
            _context.Clans.Update(clan);
            await _context.SaveChangesAsync();
            return clan;
        }

        public async Task<bool> DeleteClanAsync(Guid clanId)
        {
            var existing = await _context.Clans.FindAsync(clanId);
            if (existing == null) return false;

            _context.Clans.Remove(existing);
            await _context.SaveChangesAsync();
        
            return true;
        }
        
        public async Task<List<Clan>> GetClansByUserIdAsync(string userId)
        {
            return await _context.ClanMemberships
                .Where(cm => cm.UserId == userId)
                .Select(cm => cm.Clan)
                .Distinct()
                .ToListAsync();
        }  
    }
}
