using IdentityService.DTOs;
using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace IdentityService.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
                
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> UpdateUserAsync(UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FullName = model.FullName;       // extra property
            user.AvatarUrl = model.AvatarUrl;       // extra property

            var result = await _userManager.UpdateAsync(user);
            return result;
        }
    }
}