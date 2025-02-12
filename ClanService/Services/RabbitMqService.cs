using System;
using ClanService.Interfaces;
using Identity.DTOs;
using ClanService.Data;
using ClanService.Models;

namespace ClanService.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly ApplicationDbContext  _context;
    private readonly ILogger<RabbitMqService> _logger;  
    public RabbitMqService(ApplicationDbContext context, ILogger<RabbitMqService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task ConsumeUserInformation(UserUpdatedMessage userUpdatedMessage){
        var user = new User
        {
            Id = userUpdatedMessage.userId,
            Username = userUpdatedMessage.userName,
            Email = userUpdatedMessage.email
        };
        _context.Users.Add(user);
        try{
            await _context.SaveChangesAsync();
            _logger.LogInformation("User information saved successfully");
        }
        catch (Exception e){
            _logger.LogError(e, "Error while saving user information");       
        }
    }
}
