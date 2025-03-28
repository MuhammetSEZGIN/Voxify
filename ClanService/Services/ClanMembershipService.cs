using ClanService.Models;
using ClanService.Interfaces;

namespace ClanService.Services
{
    public class ClanMembershipService : IClanMembershipService
    {
        private readonly IClanMembershipRepository _membershipRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ClanMembershipService> _logger;
        
        public ClanMembershipService(
            IClanMembershipRepository membershipRepository,
            IUserRepository userRepository,
            ILogger<ClanMembershipService> logger)
        {
            _logger = logger;
            _membershipRepository = membershipRepository;
            _userRepository = userRepository;
        }

        public async Task<(ClanMembership, string)> AddMemberAsync(ClanMembership membership)
        {
            try  
            {
                var existingMembership = await _membershipRepository.GetByClanAndUserIdAsync(membership.ClanId, membership.UserId);
                if (existingMembership != null)
                    return (null, "User is already a member of this clan.");
                    
                var existingUser = await _userRepository.GetByIdAsync(membership.UserId);
                if (existingUser == null)
                    return (null, "User not found.");

                membership.Id = Guid.NewGuid();
                await _membershipRepository.AddAsync(membership);
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
            return await _membershipRepository.GetMembershipWithDetailsAsync(membershipId);
        }

        public async Task<List<ClanMembership>> GetMembershipsByClanIdAsync(Guid clanId)
        {
            return await _membershipRepository.GetByClanIdAsync(clanId);
        }

        public async Task<List<ClanMembership>> GetMembershipsByUserIdAsync(string userId)
        {
            return await _membershipRepository.GetByUserIdAsync(userId);
        }

        public async Task<(ClanMembership, string)> LeaveClanAsync(string userId, Guid clanId)
        {
            try
            {
                var membership = await _membershipRepository.GetByClanAndUserIdAsync(clanId, userId);

                if (membership == null)
                    return (null, "User is not a member of this clan.");

                await _membershipRepository.DeleteAsync(membership.Id);

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
            var existing = await _membershipRepository.GetByIdAsync(membershipId);
            if (existing == null) return false;

            await _membershipRepository.DeleteAsync(membershipId);
            return true;
        }
    }
}
