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
        // Check if searchText is null or empty
        // If it is, return all clans with pagination
        if(string.IsNullOrEmpty(searchText))
            return await _context.Clans
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        

        var normalizedSearchText = searchText.ToLower();
        return await _context.Clans
            .AsNoTracking()
            .Where(x =>  EF.Functions.Like(x.Name.ToLower(), $"%{normalizedSearchText}%") ||
                         EF.Functions.Like(x.Description.ToLower(), $"%{normalizedSearchText}%"))
            .OrderBy(x => x.Name)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }
}