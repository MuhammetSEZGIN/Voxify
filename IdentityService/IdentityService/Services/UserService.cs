// File: IdentityService/Services/UserService.cs
using System.Linq;
using System.Threading.Tasks;
using IdentityService.DTOs;
using IdentityService.Extensions;
using IdentityService.Interfaces;
using IdentityService.Models;
using IdentityService.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

/*
    This file just for test purposes, it is not used in the project.
 */
namespace IdentityService.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _mailService;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _config;

        public UserService(
            UserManager<ApplicationUser> userManager,
            IEmailService mailService,
            ILogger<UserService> logger,
            IConfiguration config
        )
        {
            _userManager = userManager;
            _mailService = mailService;
            _logger = logger;
            _config = config;
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
            user.AvatarUrl = model.AvatarUrl;

            if (user.Email != model.Email)
            {
                user.EmailConfirmed = false; // Email değiştiyse onay durumunu sıfırla
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "Update failed for user id {UserId}. Errors: {Errors}",
                    model.Id,
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }

            _config.GenerateJwtToken(user);

            _logger.LogInformation("Successfully updated user {UserId}", model.Id);
            return result;
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Delete failed: user id is empty");
                return IdentityResult.Failed(
                    new IdentityError { Description = "User id cannot be empty" }
                );
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
                _logger.LogWarning(
                    "Delete failed for user id {UserId}. Errors: {Errors}",
                    id,
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
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
