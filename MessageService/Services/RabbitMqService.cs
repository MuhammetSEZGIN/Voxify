using ClanService.DTOs;
using Identity.DTOs;
using MessageService.Data;
using MessageService.Interfaces.Repositories.IUserRepository;
using MessageService.Interfaces.Services;
using MessageService.Models;
namespace MessageService.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly ILogger<RabbitMqService> _logger;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    public RabbitMqService(ILogger<RabbitMqService> logger,
      IMessageRepository repository,
      IUserRepository userRepository
      )
    {
        _userRepository = userRepository;
        _messageRepository = repository;
        _logger = logger;
    }
    public async Task ConsumeUserInformation(UserUpdatedMessage userUpdatedMessage)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userUpdatedMessage.userId);
            if (user == null)
            {
                await _userRepository.AddAsync(new User
                {
                    Id = userUpdatedMessage.userId,
                    UserName = userUpdatedMessage.userName,
                    AvatarUrl = userUpdatedMessage.avatarUrl
                });
                _logger.LogInformation("New user added {0}", userUpdatedMessage.userName);
            }
            else
            {
                user.UserName = userUpdatedMessage.userName;
                user.AvatarUrl = userUpdatedMessage.avatarUrl;
                await _userRepository.UpdateAsync(userUpdatedMessage.userId,user);

                _logger.LogInformation("User updated {0}", userUpdatedMessage.userName);
            }
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

