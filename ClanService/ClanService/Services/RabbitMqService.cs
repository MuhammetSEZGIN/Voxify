using System;
using ClanService.Interfaces;
using Shared.Contracts;
using ClanService.Models;
using ClanService.Interfaces.Repositories;

namespace ClanService.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RabbitMqService> _logger;
    public RabbitMqService(IUserRepository userRepository, ILogger<RabbitMqService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }


    public async Task ConsumeUserInformation(UserUpdatedMessage userUpdatedMessage)
    {
        try
        {
            var existing = await _userRepository.GetByIdAsync(userUpdatedMessage.userId);
            var user = new User
            {
                Id = userUpdatedMessage.userId,
                Username = userUpdatedMessage.userName,
                AvatarUrl = userUpdatedMessage.AvatarUrl
            };

            if (existing != null)
            {
                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("User {UserId} updated successfully.", userUpdatedMessage.userId);
            }
            else
            {
                await _userRepository.AddAsync(user);
                _logger.LogInformation("User {UserId} created successfully.", userUpdatedMessage.userId);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while saving user information for {UserId}.", userUpdatedMessage.userId);
        }
    }
}
