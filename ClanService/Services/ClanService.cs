using ClanService.Data;
using ClanService.Models;
using ClanService.Interfaces;
using Microsoft.EntityFrameworkCore;
using ClanService.DTOs.ClanDtos;

namespace MessageService.Services
{
    public class ClanService : IClanService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClanService> _logger;

        public ClanService(ApplicationDbContext context, ILogger<ClanService> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<(Clan, string)> CreateClanAsync(Clan clan, string userId)
        {
            try
            {

                var user = await _context.Users.FindAsync(userId);
                if (user == null) 
                return (null, "User not found");

                clan.ClanId = Guid.NewGuid();
                await _context.Clans.AddAsync(clan);

                var clanMembership = new ClanMembership
                {
                    ClanId = clan.ClanId,
                    UserId = userId,
                    Role = ClanRole.Owner
                };
                await _context.ClanMemberships.AddAsync(clanMembership);

                await _context.SaveChangesAsync();
                return (clan, "Clan created successfully");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while creating clan");
                return (null, "Error while creating clan");
            }

        }

        public async Task<Clan> GetClanByIdAsync(Guid clanId)
        {
            return await _context.Clans
                .Include(c => c.Channels)
                .Include(c => c.VoiceChannels)
                .Include(c => c.ClanMemberShips).ThenInclude(cm => cm.User)
                .FirstOrDefaultAsync(c => c.ClanId == clanId);
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

        public async Task<ClanInvitation> CreateInviteTokenAsync(Guid clanId, TimeSpan? expipreInHours, int? maxUses)
        {
            
            var ClanInvitition = new ClanInvitation
            {
                ClanId = clanId,
                ExpiresAt = DateTime.UtcNow.Add(expipreInHours ?? TimeSpan.FromHours(24)),
                InviteCode = GenerateInviteCode(),
                IsActive = true,
                MaxUses = maxUses ?? 10,
                UsedCount = 0

            };
            await _context.ClanInvitations.AddAsync(ClanInvitition);
            await _context.SaveChangesAsync();
            return ClanInvitition;
        }

        private string GenerateInviteCode()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..8];
        }

        public async Task<ClanInvitation> GetInvitationByCodeAsync(string code)
        {
            return await _context.ClanInvitations
                .Include(i => i.Clan)
                .FirstOrDefaultAsync(i => i.InviteCode == code);
        }
        public async Task<(bool, string, ClanInvitation)> ValidateAndUseInvitationAsync(string code)
        {
            var invitation = await GetInvitationByCodeAsync(code);
              if (invitation == null || !invitation.IsActive)
                return (false,"The code is invalid or inactive", null);
            if (DateTime.UtcNow > invitation.ExpiresAt)
            {
                invitation.IsActive = false;
                await _context.SaveChangesAsync();
                return (false, "Expired invitation code", null);
            }
          
            if (invitation.UsedCount >= invitation.MaxUses)
            {
                invitation.IsActive = false;
                await _context.SaveChangesAsync();
                return (false, "Max usage limit reached", null);
            }

            invitation.UsedCount++;
            await _context.SaveChangesAsync();

            return (true, "Invitation code is valid", invitation);
        }
     
    }
}
