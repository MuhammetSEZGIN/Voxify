using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MessageService.Data;
using MessageService.Models;
using System.Security.Claims;

namespace MyProject.MessageService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChannelsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ChannelsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("{clanId}")]
        public async Task<IActionResult> GetChannelsInClan(int clanId)
        {
            var channels = await _db.Channels
                .Where(ch => ch.ClanId == clanId)
                .ToListAsync();
            return Ok(channels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromBody] CreateChannelRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            // Önce klan var mı kontrol et
            var clan = await _db.Clans.FindAsync(request.ClanId);
            if (clan == null)
                return NotFound("Klan bulunamadı.");

            // Kullanıcı klanın üyesi mi?
            bool isMember = await _db.ClanMemberShips
                .AnyAsync(cm => cm.ClanId == request.ClanId && cm.UserId == userId);
            if (!isMember)
                return Forbid("Klan üyesi değilsiniz.");

            var channel = new Channel
            {
                Name = request.ChannelName,
                ClanId = request.ClanId
            };
            _db.Channels.Add(channel);
            await _db.SaveChangesAsync();
            return Ok(channel);
        }
    }

    public class CreateChannelRequest
    {
        public int ClanId { get; set; }
        public string ChannelName { get; set; } = null!;
    }
}
