using ClanService.Data;
using ClanService.Interfaces.Repositories;
using ClanService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClanService.Repositories;

public class UserRepository : Repository<User, string>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<User>> GetUsersByClanIdAsync(Guid clanId)
    {
        return await _context.ClanMemberships
            .AsNoTracking()
            .Where(x => x.ClanId == clanId)
            .Select(x => x.User)
            .ToListAsync();
    }
}