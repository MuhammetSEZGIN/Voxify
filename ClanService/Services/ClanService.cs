using ClanService.Models;
using ClanService.Interfaces;
using ClanService.DTOs.ClanDtos;
using ClanService.Interfaces.Repositories;

namespace ClanService.Services
{
    public class ClanService : IClanService
    {
        private readonly IClanRepository _clanRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClanMemberShip _clanMembershipRepository;
        private readonly IClanInvitation _clanInvitationRepository;
        private readonly ILogger<ClanService> _logger;

        public ClanService(
            IClanRepository clanRepository,
            IUserRepository userRepository,
            IClanMemberShip clanMembershipRepository,
            IClanInvitation clanInvitationRepository,
            ILogger<ClanService> logger)
        {
            _logger = logger;
            _clanRepository = clanRepository;
            _userRepository = userRepository;
            _clanMembershipRepository = clanMembershipRepository;
            _clanInvitationRepository = clanInvitationRepository;
        }

        public async Task<(Clan, string)> CreateClanAsync(Clan clan, string userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) 
                    return (null, "User not found");

                clan.ClanId = Guid.NewGuid();
                await _clanRepository.AddAsync(clan);

                var clanMembership = new ClanMembership
                {
                    ClanId = clan.ClanId,
                    UserId = userId,
                    Role = ClanRole.Owner
                };
                await _clanMembershipRepository.AddAsync(clanMembership);
                
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
            return await _clanRepository.GetClanWithDetailsAsync(clanId);
        }

        public async Task<List<Clan>> GetAllClansAsync()
        {
            return await _clanRepository.GetAllAsync();
        }

        public async Task<Clan> UpdateClanAsync(Clan clan)
        {
            await _clanRepository.UpdateAsync(clan);
            return clan;
        }

        public async Task<bool> DeleteClanAsync(Guid clanId)
        {
            var existing = await _clanRepository.FindAsync(clanId);
            if (existing == null) return false;

            await _clanRepository.DeleteAsync(clanId);
            return true;
        }

        public async Task<List<Clan>> GetClansByUserIdAsync(string userId)
        {
            return await _clanRepository.GetClansByUserIdAsync(userId);
        }

        public async Task<ClanInvitation> CreateInviteTokenAsync(Guid clanId, TimeSpan? expipreInHours, int? maxUses)
        {
            var clanInvitation = new ClanInvitation
            {
                ClanId = clanId,
                ExpiresAt = DateTime.UtcNow.Add(expipreInHours ?? TimeSpan.FromHours(24)),
                InviteCode = GenerateInviteCode(),
                IsActive = true,
                MaxUses = maxUses ?? 10,
                UsedCount = 0
            };
            
            await _clanInvitationRepository.AddAsync(clanInvitation);
            return clanInvitation;
        }

        private string GenerateInviteCode()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..8];
        }

        public async Task<ClanInvitation> GetInvitationByCodeAsync(string code)
        {
            return await _clanInvitationRepository.GetByCodeAsync(code);
        }
        
        public async Task<(bool, string, ClanInvitation)> ValidateAndUseInvitationAsync(string code)
        {
            var invitation = await GetInvitationByCodeAsync(code);
            if (invitation == null || !invitation.IsActive)
                return (false,"The code is invalid or inactive", null);
                
            if (DateTime.UtcNow > invitation.ExpiresAt)
            {
                invitation.IsActive = false;
                await _clanInvitationRepository.UpdateAsync(invitation);
                return (false, "Expired invitation code", null);
            }
          
            if (invitation.UsedCount >= invitation.MaxUses)
            {
                invitation.IsActive = false;
                await _clanInvitationRepository.UpdateAsync(invitation);
                return (false, "Max usage limit reached", null);
            }

            invitation.UsedCount++;
            await _clanInvitationRepository.UpdateAsync(invitation);

            return (true, "Invitation code is valid", invitation);
        }
    }

    internal interface IClanInvitationRepository
    {
    }

    internal interface IClanMembershipRepository
    {
    }
}
