using System;
using ClanService.Interfaces;
using Identity.DTOs;
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


    public async Task ConsumeUserInformation(UserUpdatedMessage userUpdatedMessage){
        var user = new User
        {
            Id = userUpdatedMessage.userId,
            Username = userUpdatedMessage.userName,
            AvatarUrl = userUpdatedMessage.AvatarUrl
        };
        
        try{
            await _userRepository.AddAsync(user);
            _logger.LogInformation("User information saved successfully");
        }
        catch (Exception e){
            _logger.LogError(e, "Error while saving user information");       
        }
    }
}
