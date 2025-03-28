using ClanService.Data;
using ClanService.Interfaces.Repositories;
using ClanService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClanService.Repositories;

public class ClanMembershipRepository : Repository<ClanMembership, Guid>, IClanMembershipRepository
{
    public ClanMembershipRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ClanMembership>> GetMembersByClanIdAsync(Guid clanId)
    {
        return await _context.ClanMemberships
            .AsNoTracking()
            .Where(x => x.ClanId == clanId)
            .Include(x => x.User)
            .ToListAsync();
    }

    public async Task<ClanMembership> GetMemberByUserAndClanIdAsync(string userId, Guid clanId)
    {
        return await _context.ClanMemberships
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ClanId == clanId);
    }

    public async Task<bool> DeleteMembersByClanIdAsync(Guid clanId)
    {
        var members = await _context.ClanMemberships
            .Where(x => x.ClanId == clanId)
            .ToListAsync();

        if (!members.Any())
            return false;

        _context.ClanMemberships.RemoveRange(members);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ClanMembership>> GetByUserIdAsync(string userId)
    {
        return await _context.ClanMemberships
             .AsNoTracking()
             .Where(x => x.UserId == userId)
             .Include(x => x.User)
             .ToListAsync();
    }
}