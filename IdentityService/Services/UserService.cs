// File: IdentityService/Services/UserService.cs
using System.Linq;
using System.Threading.Tasks;
using IdentityService.DTOs;
using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdentityService.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IdentityResult> UpdateUserAsync(UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                _logger.LogWarning("Update failed: user with id {UserId} not found", model.Id);
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FullName = model.FullName;
            user.AvatarUrl = model.AvatarUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Update failed for user id {UserId}. Errors: {Errors}",
                    model.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            else
            {
                _logger.LogInformation("Successfully updated user {UserId}", model.Id);
            }
            return result;
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Delete failed: user id is empty");
                return IdentityResult.Failed(new IdentityError { Description = "User id cannot be empty" });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Delete failed: user with id {UserId} not found", id);
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("Successfully deleted user with id {UserId}", id);
            }
            else
            {
                _logger.LogWarning("Delete failed for user id {UserId}. Errors: {Errors}",
                    id, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return result;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("GetUser failed: empty user id");
                return null;
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found with id {UserId}", id);
                }
                return user;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user with id {UserId}", id);
                return null;
            }
        }
    }
}