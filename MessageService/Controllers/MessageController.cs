using MessageService.DTOs;
using MessageService.Interfaces;
using MessageService.Interfaces.Services;
using MessageService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MongoDB.Bson;

namespace MessageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpGet("channelId/{channelId}/clanId/{clanId}")]
        [Authorize(Roles = "OWNER,ADMIN,MEMBER")]
       public async Task<IActionResult> GetMessagesInChannelAsync(
            string channelId, 
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
                UserName = m.UserName ?? "Unknown",
                ChannelId = m.ChannelId,
                AvatarUrl = m.AvatarUrl ?? string.Empty,
                SenderId = m.SenderId,
                Text = m.Text,
                CreatedAt = m.CreatedAt
            }).ToList();
            
            return Ok(messageDtos);
        }

        [HttpDelete("/message/{messageId}/clanId/{clanId}")]
        [Authorize(Roles = "OWNER,ADMIN,MEMBER")]
        public async Task<IActionResult> DeleteMessageAsync(ObjectId messageId)
        {
            var result = await _messageService.DeleteMessageAsync(messageId);
            
            if (!result.IsSuccess)
            {
                
                return StatusCode(result.StatusCode, new { message = result.Message });
            }
            
            return Ok(new { message = "Message deleted successfully" });
        }
        [HttpPut("clanId/{clanId}")]
        [Authorize(Roles = "OWNER,ADMIN,MEMBER")]
        public async Task<IActionResult> UpdateMessage([FromBody] string message, ObjectId messageId)
        {
            var result = await _messageService.UpdateMessage(messageId, message);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }
             return Ok(new { message = "Message updated successfully" });
        }
        
    }
}
