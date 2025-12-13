using Xunit;

public class UserControllerIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public UserControllerIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void HelloWorldTest()
    {
        Assert.True(true);
    }
}
