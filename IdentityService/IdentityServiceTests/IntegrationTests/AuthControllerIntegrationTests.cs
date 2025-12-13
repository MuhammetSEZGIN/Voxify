using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Identity.DTOs;
using IdentityService.DTOs;
using IdentityService.Models;
using Xunit;

namespace IdentityServiceTests.IntegrationTests;

public class AuthControllerIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthControllerIntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    #region Register Tests

    [Fact]
    public async Task Register_ValidModel_ReturnsSuccessAndPublishesMessage()
    {
        // Arrange
        _factory.MockPublishEndpoint.Clear(); // Clear any previous messages

        var registerModel = new RegisterModel
        {
            UserName = $"testuser_{Guid.NewGuid():N}",
            Email = $"test_{Guid.NewGuid():N}@example.com",
            Password = "Password123!",
            PasswordConfirmation = "Password123!",
            DeviceInfo = "Test Device",
            AvatarUrl = "https://example.com/avatar.jpg",
        };

        var content = new StringContent(
            JsonSerializer.Serialize(registerModel),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.PostAsync("/api/Auth/register", content);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK
                || response.StatusCode == HttpStatusCode.Created
        );

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseContent);

        // Verify RabbitMQ message was published (captured by mock, not sent to real queue)
        Assert.NotEmpty(_factory.MockPublishEndpoint.PublishedMessages);

        var publishedMessage = _factory.MockPublishEndpoint.PublishedMessages.FirstOrDefault(m =>
            m is UserUpdatedMessage
        );

        Assert.NotNull(publishedMessage);

        var userMessage = publishedMessage as UserUpdatedMessage;
        Assert.NotNull(userMessage);
        Assert.Equal(registerModel.UserName, userMessage.userName);
        Assert.Equal(registerModel.AvatarUrl, userMessage.AvatarUrl);
    }

    [Fact]
    public async Task Register_InvalidEmail_DoesNotPublishMessage()
    {
        // Arrange
        _factory.MockPublishEndpoint.Clear();

        var registerModel = new RegisterModel
        {
            UserName = "testuser",
            Email = "invalid-email",
            Password = "Password123!",
            PasswordConfirmation = "Password123!",
            DeviceInfo = "Test Device",
        };

        var content = new StringContent(
            JsonSerializer.Serialize(registerModel),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.PostAsync("/api/Auth/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Verify NO message was published since registration failed
        var userMessages = _factory.MockPublishEndpoint.PublishedMessages.Where(m =>
            m is UserUpdatedMessage
        );
        Assert.Empty(userMessages);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsSuccess()
    {
        // Arrange - First register a user
        var username = $"loginuser_{Guid.NewGuid():N}";
        var password = "Password123!";

        var registerModel = new RegisterModel
        {
            UserName = username,
            Email = $"login_{Guid.NewGuid():N}@example.com",
            Password = password,
            PasswordConfirmation = password,
            DeviceInfo = "Test Device",
        };

        var registerContent = new StringContent(
            JsonSerializer.Serialize(registerModel),
            Encoding.UTF8,
            "application/json"
        );

        await _client.PostAsync("/api/Auth/register", registerContent);

        // Now login
        var loginModel = new LoginRequestModel
        {
            UserName = username,
            Password = password,
            DeviceInfo = "Test Device",
        };

        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginModel),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.PostAsync("/api/Auth/login", loginContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<AuthResponseDto>>(
            responseContent,
            _jsonOptions
        );

        Assert.NotNull(result);
        Assert.True(result.IsSuccessfull);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.AccessToken);
        Assert.NotNull(result.Data.RefreshToken);
        Assert.NotNull(result.Data.UserID);
    }

    #endregion
}
