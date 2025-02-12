using System;

namespace Identity.DTOs;

public record UserUpdatedMessage
{
    public string userId { get; init; }
    public string userName { get; init; }
    public string email { get; init; }

}
