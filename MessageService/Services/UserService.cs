using MessageService.Interfaces.Repositories.IUserRepository;
using MessageService.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public Task<string> GetUserNameByIdAsync(string userId)
        {
            return _userRepository.GetByIdAsync(userId).ContinueWith(task =>
            {
                var user = task.Result;
                return user != null ? user.UserName : null;
            });
        }
    }
}
