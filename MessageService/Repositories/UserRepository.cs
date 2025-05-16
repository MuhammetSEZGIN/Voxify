using System;
using MessageService.Data;
using MessageService.Interfaces.Repositories.IUserRepository;
using MessageService.Models;
using MessageService.Repositories.Base;

namespace MessageService.Repositories;

public class UserRepository : Repository<User, string>, IUserRepository
{
    IMongoDbContext _context;
    public UserRepository(IMongoDbContext context, string collectionName) : base(context, collectionName)
    {
        _context = context;
    }
   
}
