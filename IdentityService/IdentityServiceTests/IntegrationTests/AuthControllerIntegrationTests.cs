using Xunit;
using System.Net.Http;
using System.Threading.Tasks;

public class AuthControllerIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(DatabaseFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task AuthController_HelloWorld_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/api/auth/hello");
        response.EnsureSuccessStatusCode();
    }
}