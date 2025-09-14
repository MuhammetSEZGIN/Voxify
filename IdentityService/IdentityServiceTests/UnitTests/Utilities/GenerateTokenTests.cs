using Xunit;

public class GenerateTokenTests
{
    [Fact]
    public void GenerateToken_ReturnsValidToken()
    {
        var token = TokenUtility.GenerateToken("testUser");
        Assert.NotNull(token);
        Assert.True(token.Length > 0);
    }
}