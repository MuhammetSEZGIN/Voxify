using Identity.DTOs;
using MessageService.Data;
using MessageService.Interfaces;
using MessageService.Models;
namespace MessageService.Services;

public class RabbitMqService : IRabbitMqService
{
    ILogger<RabbitMqService> _logger;
    ApplicationDbContext _context;
    public RabbitMqService(ILogger<RabbitMqService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task ConsumeUserInformation(UserUpdatedMessage message)
    {
        var user = await _context.Users.FindAsync(message.userId);
        if (user == null)
        {
            await _context.Users.AddAsync(new User
            {
                Id = message.userId,
                UserName = message.userName,
                AvatarUrl = message.avatarUrl
            });
            _logger.LogInformation("New user added {0}", message.userName);
        }else{
            user.UserName = message.userName;
            user.AvatarUrl = message.avatarUrl;
            _context.Users.Update(user);
            
            _logger.LogInformation("User updated {0}", message.userName);
        }
        try{
            await _context.SaveChangesAsync();
        }catch(Exception e){
            _logger.LogError(e.Message);
        }
    }
   
 
}
