using System;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using IdentityService.Models;
using IdentityService.Interfaces;
using IdentityService.Messaging;
namespace IdentityService.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;
    private readonly IdentityProducer _messagePublisher;
    public AuthService(UserManager<ApplicationUser> userManager,
                        IConfiguration config,
                        IdentityProducer messagePublisher,
                        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _config = config;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }


    public async Task<IdentityResult> RegisterAsync(RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FullName = model.FullName,
            AvatarUrl = model.AvatarUrl
        };
        var result = await _userManager.CreateAsync(user, model.Password!);
        if (result.Succeeded)
        {
            await _messagePublisher.PublishUserUpdatedMessageAsync(
               user.UserName,
                user.AvatarUrl,
                user.Id
            );
            _logger.LogInformation(
                "User created. Username: {Username}, Email: {Email}, FullName: {FullName}, AvatarUrl: {AvatarUrl}",
                model.UserName,
                model.Email,
                model.FullName,
                model.AvatarUrl
);
        }
        return result;
    }
    public async Task<string> LoginAsync(LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName!);
        if (user == null)
        {
            _logger.LogWarning("User not found with username: {0}", model.UserName);
            return "Invalid login attempt";
        }
        var checkedPassword = await _userManager.CheckPasswordAsync(user, model.Password!);
        if (!checkedPassword)
        {
            _logger.LogWarning("Invalid password for user: {0}", model.UserName);
            return "Invalid login attempt";
        }
        _logger.LogInformation("User logged in: {0}", model.UserName);
        return GenerateJSONWebToken(user);
    }
    public string GenerateJSONWebToken(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JWT:Key"]!);  //config dosyasından okuyoruz
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "Avel")
            };
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddHours(2),
        signingCredentials: creds
        );
        return tokenHandler.WriteToken(token);
    }

}
