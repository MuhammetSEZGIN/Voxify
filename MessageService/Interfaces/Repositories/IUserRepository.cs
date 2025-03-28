using System;
using MessageService.Models;

namespace MessageService.Interfaces.Repositories.IUserRepository;
public interface IUserRepository :IRepository<User, string>
{

}
