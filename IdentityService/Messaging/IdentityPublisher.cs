using System;
using Identity.DTOs;
using MassTransit;

namespace IdentityService.Messaging;


public class IdentityProducer
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<IdentityProducer> _logger;

    public IdentityProducer(IPublishEndpoint publishEndpoint, ILogger<IdentityProducer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishUserUpdatedMessageAsync(string userName, string email, string userId)
    {
        var message = new UserUpdatedMessage
        {
            userId = userId,
            userName = userName,
            email = email
        };

        _logger.LogInformation("Publishing UserUpdatedMessage for user: {userName}, email: {email}", userName, email);
        await _publishEndpoint.Publish(message);
    }
}


