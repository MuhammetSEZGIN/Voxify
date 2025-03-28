using System;
using ClanService.Data;
using ClanService.Interfaces.Repositories;
using ClanService.Models;
using ClanService.Services;
using Microsoft.EntityFrameworkCore;

namespace ClanService.Repositories;

public class ClanInvitationRepository : Repository<ClanInvitation, Guid>, IClanInvitation
{
    public ClanInvitationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ClanInvitation> GetByCodeAsync(string code)
    {
        return await _context.ClanInvitations.
        Include(a=>a.Clan)
        .FirstOrDefaultAsync(a=>a.InviteCode.Equals(code));
    }
}
