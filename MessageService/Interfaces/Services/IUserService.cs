using System;
using MessageService.Models;

namespace MessageService.Interfaces.Services;

public interface IUserService
{
    Task<string> GetUserNameByIdAsync(string userId);
}
