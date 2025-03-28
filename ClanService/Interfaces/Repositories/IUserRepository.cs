using System;
using ClanService.Models;

namespace ClanService.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, string>
{
    Task<IEnumerable<User>> GetUsersByClanIdAsync(Guid clanId);
}