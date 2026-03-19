using MessageService.Interfaces.Repositories.IUserRepository;
using MessageService.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public async Task<string> GetUserNameByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.UserName;
        }
    }
}
