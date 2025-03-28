using System;
using MessageService.Data;
using MessageService.Interfaces.Repositories.IUserRepository;
using MessageService.Models;
using MessageService.Repositories.Base;

namespace MessageService.Repositories;

public class UserRepository : Repository<User, string>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
}
