// File: ClanService/Controllers/UserController.cs
using Microsoft.AspNetCore.Mvc;
using ClanService.Data;
using ClanService.Models;

/*
Sadece test amaçlı oluşturulmuş bir controller.
Kullanıcı oluşturmayı ve kullanıcı bilgilerini getirmeyi sağlar.
Normalde IdentityServer kullanılmalıdır.
*/


namespace ClanService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/User/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User data is required.");

            // auto-generate an Id if none is provided
            if (string.IsNullOrWhiteSpace(user.Id))
            {
                user.Id = Guid.NewGuid().ToString();
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }
    }
}