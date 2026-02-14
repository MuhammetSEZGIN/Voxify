using System;

namespace IdentityService.Interfaces;

public interface IIdentityProducer
{
    Task PublishUserUpdatedMessageAsync(
            string userName,
            string avatarUrl,
            string userId
        );
}
