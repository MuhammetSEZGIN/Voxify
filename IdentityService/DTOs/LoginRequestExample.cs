using System;
using IdentityService.Models;
using Swashbuckle.AspNetCore.Filters;

namespace IdentityService.Examples;

public class LoginRequestExample : IExamplesProvider<LoginRequestModel>
{
    public LoginRequestModel GetExamples()
    {
        return new LoginRequestModel
        {
            UserName = "exampleUser",
            Password = "Password123!",
            DeviceInfo = "Device XYZ",
        };
    }
}
