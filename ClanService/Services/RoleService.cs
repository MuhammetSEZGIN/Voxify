using System;
using ClanService.Interfaces;

namespace ClanService.Services;

public class RoleService : IRoleService
{
    private readonly IClanMembershipRepository _membershipRepository;
    private readonly ILogger<RoleService> _logger;
    
    public RoleService(IClanMembershipRepository membershipRepository, ILogger<RoleService> logger)
    {
        _logger = logger;
        _membershipRepository = membershipRepository;
    }
    
    public async Task<bool> UpdateRoleAsync(Guid membershipId, string roleName)
    {
        try
        {
            var existingMembership = await _membershipRepository.GetByIdAsync(membershipId);
            if (existingMembership == null)
            {
                _logger.LogWarning("Membership not found.");
                return false;
            }
            
            existingMembership.Role = roleName;
            await _membershipRepository.UpdateAsync(existingMembership);
            _logger.LogInformation("Role of membership {MembershipId} updated to {RoleName}.", membershipId, roleName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating role of membership {MembershipId}.", membershipId);
            return false;
        }
    }
}
