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
    [Authorize] // JWT zorunlu
    public class ClansController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ClansController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetClans()
        {
            var clans = await _db.Clans.Include(c => c.Channels).ToListAsync();
            return Ok(clans);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClan([FromBody] string clanName)
        {
            var clan = new Clan { Name = clanName };
            _db.Clans.Add(clan);
            await _db.SaveChangesAsync();
            return Ok(clan);
        }

        // Kullanıcıyı klana üye yap
        [HttpPost("{clanId}/join")]
        public async Task<IActionResult> JoinClan(int clanId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Token geçersiz.");

            // Klan var mı?
            var clan = await _db.Clans.FindAsync(clanId);
            if (clan == null)
                return NotFound("Klan bulunamadı.");

            // Zaten üyeyse tekrar ekleme
            bool alreadyMember = await _db.ClanMemberShips
                .AnyAsync(cm => cm.ClanId == clanId && cm.UserId == userId);
            if (alreadyMember)
                return BadRequest("Kullanıcı zaten bu klana üye.");

            var membership = new ClanMemberShip
            {
                ClanId = clanId,
                UserId = userId
            };
            _db.ClanMemberShips.Add(membership);
            await _db.SaveChangesAsync();
            return Ok(membership);
        }
    }
}
