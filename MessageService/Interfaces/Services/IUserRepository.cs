using System;
using MessageService.Models;

namespace MessageService.Interfaces.Services;

public interface IUserRepository : IRepository<User, Guid>
{

}
