using System.Net;
using IdentityService.DTOs;
using IdentityService.Interfaces;
using IdentityService.Models;
using IdentityService.Utilities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;

namespace IdentityService.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIpAddressService _ipAddressService;
    private readonly IRefreshTokenService _refreshTokenService;

    public EmailService(
        IConfiguration configuration,
        ILogger<EmailService> logger,
        UserManager<ApplicationUser> userManager,
        IIpAddressService ipAddressService,
        IRefreshTokenService refreshTokenService
    )
    {
        _configuration = configuration;
        _logger = logger;
        _userManager = userManager;
        _ipAddressService = ipAddressService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<ApiResponse<object>> SendEmailAsync(
        string toEmail,
        string subject,
        string content
    )
    {
        try
        {
            // appsettings veya User Secrets'tan SMTP ayarlarını çek
            var smtpSettings = _configuration.GetSection("Smtp");
            var isEnabled = smtpSettings.GetValue<bool>("Enabled", true);
            
            // Eğer mail servisi devre dışı ise, işlem yapmadan başarılı response döndür
            if (!isEnabled)
            {
                _logger.LogInformation("Mail servisi devre dışı. E-posta gönderilmedi: {Email}", toEmail);
                return ApiResponse<object>.Success(
                    "E-posta servisi devre dışı.",
                    (int)HttpStatusCode.OK
                );
            }
            var fromAddress = smtpSettings["FromAddress"];
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"]);
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];

            // MimeMessage nesnesini oluştur (e-postanın kendisi)
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(fromAddress));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = content };

            // SmtpClient nesnesini oluştur ve bağlan
            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(username, password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }

            _logger.LogInformation("MailKit ile e-posta başarıyla gönderildi: {Email}", toEmail);
            return ApiResponse<object>.Success(
                "E-posta başarıyla gönderildi.",
                (int)HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "MailKit ile e-posta gönderilirken bir hata oluştu. Alıcı: {Email}",
                toEmail
            );
            return ApiResponse<object>.Failed(
                "E-posta gönderilirken bir hata oluştu.",
                new[] { ex.Message },
                (int)HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ApiResponse<object>> ConfirmEmail(string userId, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("User ID or token is null or empty");
                return ApiResponse<object>.Failed("User ID or token is null or empty");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {0}", userId);
                return ApiResponse<object>.Failed("User not found");
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Email already confirmed for user: {0}", user.UserName);
                return ApiResponse<object>.Success(
                    "Email already confirmed.",
                    (int)HttpStatusCode.OK
                );
            }
            var decodedToken = WebUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Email confirmation failed for user: {0}", user.UserName);
                return ApiResponse<object>.Failed(
                    "Email confirmation failed.",
                    result.Errors.Select(e => e.Description)
                );
            }
            _logger.LogInformation("Email confirmed for user: {0}", user.UserName);
            var refreshTokenResult = await _refreshTokenService.CreateUserRefreshTokenAsync(
                user.Id,
                "Email Confirmation Device", // Default device info
                _ipAddressService.GetClientIpAddress()
            );
            if (!refreshTokenResult.IsSuccessfull)
            {
                _logger.LogWarning("Failed to create refresh token after email confirmation for user: {0}", user.UserName);
                return ApiResponse<object>.Failed(
                    "Email confirmed but failed to create session tokens.",
                    refreshTokenResult.Errors,
                    refreshTokenResult.StatusCode
                );
            }
            return ApiResponse<object>.Success(
                new { AccessToken = refreshTokenResult.Data.AccessToken, RefreshToken = refreshTokenResult.Data.RefreshToken },
                "Email confirmed successfully."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while confirming email for user ID: {UserId}",
                userId
            );
            return ApiResponse<object>.Failed(
                "An error occurred while confirming email.",
                new List<string> { ex.Message },
                (int)HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ApiResponse<object>> SendEmailConfirmationAsync(
        string userId,
        string confirmationUrl
    )
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {0}", userId);
                return ApiResponse<object>.Failed("User not found");
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Email already confirmed for user: {0}", user.UserName);
                return ApiResponse<object>.Success(
                    "Email already confirmed.",
                    (int)HttpStatusCode.OK
                );
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var encodedUserId = WebUtility.UrlEncode(userId);

            var fullConfirmationUrl =
                $"{confirmationUrl}?userId={encodedUserId}&token={encodedToken}";

            var subject = "Email Confirmation";
            var htmlContent =
                $@"
            <h2>Welcome {user.UserName}!</h2>
            <p>Please confirm your email address by clicking the link below:</p>
            <a href='{fullConfirmationUrl}' style='background-color: #4CAF50; color: white; padding: 14px 20px; text-decoration: none; display: inline-block; border-radius: 4px;'>
                Confirm Email
            </a>
            <p>If the button doesn't work, copy and paste this link into your browser:</p>
            <p>{fullConfirmationUrl}</p>
            <p>This link will expire in 24 hours.</p>
        ";

            var emailResult = await SendEmailAsync(user.Email, subject, htmlContent);
            if (emailResult.IsSuccessfull)
            {
                _logger.LogInformation(
                    "Email confirmation sent successfully to: {Email}",
                    user.Email
                );
                return ApiResponse<object>.Success(
                    "Confirmation email sent successfully. Please check your inbox.",
                    (int)HttpStatusCode.OK
                );
            }
            else
            {
                _logger.LogError("Failed to send confirmation email to: {Email}", user.Email);
                return ApiResponse<object>.Failed(
                    "Failed to send confirmation email. Please try again later.",
                    emailResult.Errors,
                    (int)HttpStatusCode.InternalServerError
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email confirmation to user: ");
            return ApiResponse<object>.Failed(
                "Failed to send confirmation email.",
                new[] { ex.Message },
                (int)HttpStatusCode.InternalServerError
            );
        }
    }
}
