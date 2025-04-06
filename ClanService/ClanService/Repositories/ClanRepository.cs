using ClanService.Data;
using ClanService.Interfaces.Repositories;
using ClanService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClanService.Repositories;

public class ClanRepository : Repository<Clan, Guid>, IClanRepository
{
    public ClanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Clan>> GetClansByUserIdAsync(string id)
    {
        return await _context.ClanMemberships
                .Where(cm => cm.UserId.Equals(id))
                .Select(cm => cm.Clan)
                .Distinct()
                .ToListAsync(); 
    }

    public async Task<Clan> GetClanWithDetailsAsync(Guid id)
    {
         return await _context.Clans
                .Include(c => c.Channels)
                .Include(c => c.VoiceChannels)
                .Include(c => c.ClanMemberShips).ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync(c => c.ClanId == id);   
    }

    public async Task<IEnumerable<Clan>> SearchClansAsync(string searchText, int limit, int page)
    {
        return await _context.Clans
            .AsNoTracking()
            .Where(x => x.Name.Contains(searchText) || x.Description.Contains(searchText))
            .OrderBy(x => x.Name)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }
}