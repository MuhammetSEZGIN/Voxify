using System;
using ClanService.Models;

namespace ClanService.Interfaces.Repositories;

public interface IClanInvitation : IRepository<ClanInvitation, Guid>
{
    Task<ClanInvitation> GetByCodeAsync(string code);
}
