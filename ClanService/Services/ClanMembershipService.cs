using ClanService.Data;
using ClanService.Models;
using ClanService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClanService.Services
{
    public class ClanMembershipService : IClanMembershipService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClanMembershipService> _logger;
        public ClanMembershipService(ApplicationDbContext context, ILogger<ClanMembershipService> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<(ClanMembership, string)> AddMemberAsync(ClanMembership membership)
        {
            try  
            {
                var existingMembership = await _context.ClanMemberships.AsNoTracking()
                .FirstOrDefaultAsync(cm => cm.ClanId == membership.ClanId && cm.UserId == membership.UserId);
                if (existingMembership != null)
                    return (null, "User is already a member of this clan.");
                var existingUser = await _context.Users.FindAsync(membership.UserId);
                if (existingUser == null)
                    return (null, "User not found.");

                membership.Id = Guid.NewGuid();
                await _context.ClanMemberships.AddAsync(membership);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User {UserId} added to clan {ClanId} successfully.", membership.UserId, membership.ClanId);
                return (membership, "User added to the clan successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding user {UserId} to clan {ClanId}.", membership.UserId, membership.ClanId);
                return (null, "An error occurred while adding user to the clan.");
            }


        }

        public async Task<ClanMembership> GetMembershipAsync(Guid membershipId)
        {
            return await _context.ClanMemberships.AsNoTracking()
                .Include(cm => cm.Clan) // Gerekirse clan bilgilerini de dahil edebilirsiniz
                .FirstOrDefaultAsync(cm => cm.Id == membershipId);
        }

        public async Task<List<ClanMembership>> GetMembershipsByClanIdAsync(Guid clanId)
        {
            return await _context.ClanMemberships.AsNoTracking()
                .Where(cm => cm.ClanId == clanId)
                .ToListAsync();
        }

        public async Task<List<ClanMembership>> GetMembershipsByUserIdAsync(string userId)
        {
            return await _context.ClanMemberships.AsNoTracking()
                .Where(cm => cm.UserId == userId)
                .ToListAsync();
        }

        public async Task<(ClanMembership, string)> LeaveClanAsync(string userId, Guid clanId)
        {
            try
            {
                var membership = await _context.ClanMemberships
                    .FirstOrDefaultAsync(cm => cm.ClanId == clanId && cm.UserId == userId);

                if (membership == null)
                    return (null, "User is not a member of this clan.");

                _context.ClanMemberships.Remove(membership);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} has left clan {ClanId} successfully.", userId, clanId);
                return (membership, "User has left the clan successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while user {UserId} was leaving clan {ClanId}.", userId, clanId);
                return (null, "An error occurred while leaving the clan.");
            }
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
