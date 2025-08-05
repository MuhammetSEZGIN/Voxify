using System;
using IdentityService.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace IdentityService.Examples;

public class RegisterRequestExample : IExamplesProvider<RegisterModel>
{
    public RegisterModel GetExamples()
    {
        return new RegisterModel
        {
            UserName = "exampleUser",
            Email = "example@example.com",
            Password = "Password123!",
            PasswordConfirmation = "Password123!",
            DeviceInfo = "Device XYZ",
            AvatarUrl =
                "https://gravatar.com/avatar/274630e59be6809935f57613a27a0c5f?s=400&d=robohash&r=x",
        };
    }
}
