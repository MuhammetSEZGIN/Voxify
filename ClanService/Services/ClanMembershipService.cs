using ClanService.Data;
using ClanService.Models;
using ClanService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClanService.Services
{
    public class ClanMembershipService : IClanMembershipService
    {
        private readonly ApplicationDbContext _context;

        public ClanMembershipService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(ClanMembership, string)> AddMemberAsync(ClanMembership membership)
        {
            var existingMembership= await _context.ClanMemberships
                .FirstOrDefaultAsync(cm => cm.ClanId == membership.ClanId && cm.UserId == membership.UserId);
            if (existingMembership != null) 
                return (null, "User is already a member of this clan.");

            membership.Id = Guid.NewGuid(); 
            await _context.ClanMemberships.AddAsync(membership);
            await _context.SaveChangesAsync();
            return (membership, "User added to the clan successfully.");
        }

        public async Task<ClanMembership> GetMembershipAsync(Guid membershipId)
        {
            return await _context.ClanMemberships
                .Include(cm => cm.Clan) // Gerekirse clan bilgilerini de dahil edebilirsiniz
                .FirstOrDefaultAsync(cm => cm.Id == membershipId);
        }

        public async Task<List<ClanMembership>> GetMembershipsByClanIdAsync(Guid clanId)
        {
            return await _context.ClanMemberships
                .Where(cm => cm.ClanId == clanId)
                .ToListAsync();
        }

        public async Task<List<ClanMembership>> GetMembershipsByUserIdAsync(string userId)
        {
            return await _context.ClanMemberships
                .Where(cm => cm.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> RemoveMemberAsync(Guid membershipId)
        {
            var existing = await _context.ClanMemberships.FindAsync(membershipId);
            if (existing == null) return false;

            _context.ClanMemberships.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
