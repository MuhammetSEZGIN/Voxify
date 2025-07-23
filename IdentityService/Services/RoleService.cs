using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RoleService> logger
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogWarning("CreateRoleAsync: Role '{Role}' already exists", roleName);
                return IdentityResult.Failed(
                    new IdentityError { Description = "Role already exists." }
                );
            }

            return await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        public async Task<IdentityResult> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                _logger.LogWarning("DeleteRoleAsync: Role '{Role}' not found", roleName);
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
            }

            return await _roleManager.DeleteAsync(role);
        }

        public async Task<IdentityResult> AssignUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning(
                    "AssignUserToRoleAsync: User with ID '{UserId}' not found",
                    userId
                );
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            // Check if user is already in the role.
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                _logger.LogWarning(
                    "AssignUserToRoleAsync: User '{UserId}' is already in role '{Role}'",
                    userId,
                    roleName
                );
                return IdentityResult.Success;
            }

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("GetUserRolesAsync: User with ID '{UserId}' not found", userId);
                return new List<string>();
            }
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("IsUserInRoleAsync: User with ID '{UserId}' not found", userId);
                return false;
            }
            return await _userManager.IsInRoleAsync(user, roleName);
        }
    }
}
