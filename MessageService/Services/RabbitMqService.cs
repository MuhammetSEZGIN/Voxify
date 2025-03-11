using ClanService.DTOs;
using Identity.DTOs;
using MessageService.Data;
using MessageService.Interfaces;
using MessageService.Interfaces.Services;
using MessageService.Models;
namespace MessageService.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly ILogger<RabbitMqService> _logger;
    private readonly ApplicationDbContext _context;

    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    public RabbitMqService(ILogger<RabbitMqService> logger,
     ApplicationDbContext context,
      IMessageRepository repository,
      IUserRepository userRepository
      )
    {
        _userRepository = userRepository;
        _messageRepository = repository;
        _logger = logger;
        _context = context;
    }
    public async Task ConsumeUserInformation(UserUpdatedMessage message)
    {
        var user = await _userRepository.GetByIdAsync(message.userId);
        if (user == null)
        {
            await _userRepository.AddAsync(new User
            {
                Id = message.userId,
                UserName = message.userName,
                AvatarUrl = message.avatarUrl
            });
            _logger.LogInformation("New user added {0}", message.userName);
        }
        else
        {
            user.UserName = message.userName;
            user.AvatarUrl = message.avatarUrl;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User updated {0}", message.userName);
        }
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
    public async Task ConsumeChannelInformation(ChannelDeletedMessage message)
    {
        var result = await _messageRepository.DeleteMessagesOfChannelByChannelId(message.ChannelId);
        if (result)
        {
            _logger.LogInformation("Messages deleted for channel {0}", message.ChannelId);
        }
        else
        {
            _logger.LogInformation("No messages found for channel {0}", message.ChannelId);
        }   
    }

}

