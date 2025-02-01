using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessageService.Data;
using MessageService.Models;

namespace MyProject.MessageService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public MessagesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Bir kanaldaki son X mesajı çekmek
        [HttpGet("channel/{channelId}")]
        public async Task<IActionResult> GetChannelMessages(int channelId, [FromQuery] int limit = 50)
        {
            var messages = await _db.Messages
                .Where(m => m.ChannelId == channelId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync();
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
