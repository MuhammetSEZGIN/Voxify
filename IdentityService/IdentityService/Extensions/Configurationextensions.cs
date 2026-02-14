using System;
using IdentityService.Models;
using IdentityService.Utilities;

namespace IdentityService.Extensions;

public static class Configurationextensions
{
    public static string GenerateJwtToken(this IConfiguration config, ApplicationUser user)
    {
        return GenerateToken.GenerateJSONWebToken(user, config);
    }

    public static int GetJwtExpireHours(this IConfiguration config)
    {
        return int.Parse(config["JWT:ExpireHours"] ?? "2");
    }
}
