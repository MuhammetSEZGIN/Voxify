using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessageService.Data;
using MessageService.Models;
using MessageService.Interfaces;

namespace MyProject.MessageService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesController> _logger;
        public MessagesController(ApplicationDbContext db,
                                IMessageService messageService,
                                ILogger<MessagesController> logger)
        {
            _db = db;
            _messageService = messageService;
            _logger = logger;
        }
        

        // Bir kanaldaki son X mesajı çekmek
        [HttpGet("channel/{channelId}")]
        public async Task<IActionResult> GetChannelMessages(int channelId, [FromQuery] int limit = 50)
        {
            var messages = await _messageService.GetMessagesInChannelAsync(channelId, limit);
            if (messages == null)
            {
                _logger.LogWarning("Channel not found with id: {0}", channelId);
                return NotFound();
            }
            _logger.LogInformation("Messages in channel {0} fetched successfully", channelId);
            return Ok(messages);
        }

        // Birebir mesaj geçmişi
        [HttpGet("direct/{otherUserId}")]
        public async Task<IActionResult> GetDirectMessages(string otherUserId, [FromQuery] int limit = 50)
        {
            var currentUserId = User.Identity?.Name; // veya ClaimTypes.NameIdentifier
            if (currentUserId == null)
                return Unauthorized();

            var messages = await _db.Messages
                .Where(m =>
                    (m.SenderId == currentUserId && m.RecipientId == otherUserId) ||
                    (m.SenderId == otherUserId && m.RecipientId == currentUserId)
                )
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync();

            return Ok(messages);
        }
    }
}
