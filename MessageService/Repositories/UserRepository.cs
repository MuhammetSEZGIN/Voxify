using System;
using MessageService.Data;
using MessageService.Interfaces;
using MessageService.Models;
using MessageService.Repositories.Base;

namespace MessageService.Repositories;

public class UserRepository : Repository<User, string>
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
}
