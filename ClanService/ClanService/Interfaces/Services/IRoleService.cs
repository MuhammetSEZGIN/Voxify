using System;

namespace ClanService.Interfaces;

public interface IRoleService
{
    public Task<bool> UpdateRoleAsync(Guid membershipId, string roleName);    
}

