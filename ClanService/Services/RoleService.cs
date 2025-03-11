using System;
using ClanService.Data;
using ClanService.Interfaces;

namespace ClanService.Services;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoleService> _logger;
    public RoleService(ApplicationDbContext context, ILogger<RoleService> logger)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<bool> UpdateRoleAsync(Guid membershipId, string roleName)
    {
        try
        {
            var existingMembership = await _context.ClanMemberships.FindAsync(membershipId);
            if (existingMembership == null)
            {
                _logger.LogWarning("Membership not found.");
                return false;
            }
            existingMembership.Role = roleName;
            _context.ClanMemberships.Update(existingMembership);
            await _context.SaveChangesAsync();
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
