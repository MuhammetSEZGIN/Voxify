using MessageService.DTOs;
using MessageService.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MessageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    [EnableRateLimiting("fixed")]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
       public async Task<IActionResult> GetMessagesInChannelAsync(
            [FromQuery] Guid channelId, 
            [FromQuery] int limit = 20, 
            [FromQuery] int page = 1)
        {
            var result = await _messageService.GetMessagesInChannelAsync(channelId, limit, page);
            
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }

            var messageDtos = result.Data.Select(m => new MessageDto 
            {
                Id = m.Id,
                UserName = m.User?.UserName ?? "Unknown",
                ChannelId = m.ChannelId,
                AvatarUrl = m.User?.AvatarUrl ?? string.Empty,
                SenderId = m.SenderId,
                Text = m.Text,
                CreatedAt = m.CreatedAt
            }).ToList();
            
            return Ok(messageDtos);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMessageAsync(Guid messageId)
        {
            var result = await _messageService.DeleteMessageAsync(messageId);
            
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }
            
            return Ok(new { message = "Message deleted successfully" });
        }

    }
}
