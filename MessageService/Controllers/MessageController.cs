using MessageService.DTOs;
using MessageService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesInChannelAsync(Guid channelId, int limit)
        {
            var messages = await _messageService.GetMessagesInChannelAsync(channelId, limit);
            var messageDtos = messages.Select(m => new MessageDto 
            {
                Id = m.Id,
                UserName = m.User != null ? m.User.UserName : string.Empty,
                ChannelId = m.ChannelId,
                AvatarUrl = m.User != null ? "https://www.pngwing.com/en/search?q=user+Avatar" : string.Empty,
                SenderId = m.SenderId,
                Text = m.Text,
                CreatedAt = m.CreatedAt
            }).ToList();
            return Ok(messageDtos);
        }   

    }
}
