using Microsoft.AspNetCore.Mvc;
using ClanService.Interfaces;
using ClanService.Models;
using ClanService.DTOs;
using AutoMapper;

namespace ClanService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelService _channelService;
        private readonly IMapper _mapper;

        public ChannelController(IChannelService channelService, IMapper mapper)
        {
            _channelService = channelService;
            _mapper = mapper;
        }


        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromBody] ChannelCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto
                {
                    Message = "Invalid channel data.",
                    Errors = ModelState
                });
            var channel = _mapper.Map<Channel>(dto);
            var created = await _channelService.CreateChannelAsync(channel);

            var readDto = _mapper.Map<ChannelReadDto>(created);
            return Ok(readDto);
        }

        [HttpGet("{channelId}")]
        public async Task<IActionResult> GetChannelById(Guid channelId)
        {
            var channel = await _channelService.GetChannelByIdAsync(channelId);
            if (channel == null)
                return NotFound(new ErrorDto
                {
                    Message = "Channel not found."
                });

            var readDto = _mapper.Map<ChannelReadDto>(channel);
            return Ok(readDto);
        }

        [HttpGet("clan/{clanId}")]
        public async Task<IActionResult> GetChannelsByClanId(Guid clanId)
        {
            var channels = await _channelService.GetChannelsByClanIdAsync(clanId);
            if (channels == null)
                return NotFound(new ErrorDto
                {
                    Message = "Channels not found."
                });
            var readDtoList = _mapper.Map<List<ChannelReadDto>>(channels);
            return Ok(readDtoList);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChannel([FromBody] ChannelUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto
                {
                    Message = "Invalid channel data.",
                    Errors = ModelState
                });

            var existing = await _channelService.GetChannelByIdAsync(dto.ChannelId);
            if (existing == null)
                return NotFound("Channel not found.");

            existing.Name = dto.Name;

            var updated = await _channelService.UpdateChannelAsync(existing);
            var readDto = _mapper.Map<ChannelReadDto>(updated);

            return Ok(readDto);
        }

        [HttpDelete("{channelId}")]
        public async Task<IActionResult> DeleteChannel(Guid channelId)
        {
            var result = await _channelService.DeleteChannelAsync(channelId);
            if (!result)
                return NotFound(new ErrorDto
                {
                    Message = "Channel not found or already deleted."
                });
            return NoContent();
        }



    }
}
