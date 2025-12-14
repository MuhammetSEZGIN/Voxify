using Xunit;
using IdentityService.Utilities;
using IdentityService.Models;
using Microsoft.Extensions.Configuration;
public class GenerateTokenTests
{
    [Fact]
    public void GenerateToken_ReturnsValidToken()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "JWT:Key", "test-signing-key-12345678901234567890" }
            })
            .Build();
     
          var testUser = new ApplicationUser
        {
            Id = "user-123",
            UserName = "testuser",
            EmailConfirmed = true,
            AvatarUrl = "https://example.com/avatar.png"
        };
        var token = GenerateToken.GenerateJSONWebToken(testUser, config);
        Assert.NotNull(token);
        Assert.True(token.Length > 0);
    }
}