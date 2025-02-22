using Microsoft.AspNetCore.Mvc;
using ClanService.Interfaces;
using ClanService.Models;
using ClanService.DTOs;
using AutoMapper;

namespace ClanService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClanController : ControllerBase
    {
        private readonly IClanService _clanService;
        private readonly IMapper _mapper;
        
        public ClanController(IClanService clanService, IMapper mapper)
        {
            _clanService = clanService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateClan([FromBody] ClanCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto 
                { 
                    Message = "Invalid clan data.", 
                    Errors = ModelState 
                });

            var clan = _mapper.Map<Clan>(dto);
            var (created, message) = await _clanService.CreateClanAsync(clan, dto.UserId);
            if (created == null)
                return BadRequest(new ErrorDto { Message = message });
            var readDto = _mapper.Map<ClanReadDto>(created);
            return Ok(readDto);
        }

        [HttpGet("{clanId}")]
        public async Task<IActionResult> GetClanById(Guid clanId)
        {
            var clan = await _clanService.GetClanByIdAsync(clanId);
            if (clan == null)
                return NotFound(new ErrorDto { Message = "Clan not found." });

            var readDto = _mapper.Map<GetAllClanPropertyDto>(clan);
            return Ok(readDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClans()
        {
            var clans = await _clanService.GetAllClansAsync();
            var readDtoList = _mapper.Map<List<ClanReadDto>>(clans);
            return Ok(readDtoList);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClan([FromBody] ClanUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto 
                { 
                    Message = "Invalid clan data.", 
                    Errors = ModelState 
                });

            var existing = await _clanService.GetClanByIdAsync(dto.ClanId);
            if (existing == null)
                return NotFound(new ErrorDto { Message = "Clan not found." });

            existing.Name = dto.Name;
            existing.ImagePath = dto.ImagePath;
            
            var updated = await _clanService.UpdateClanAsync(existing);
            var readDto = _mapper.Map<ClanReadDto>(updated);
            return Ok(readDto);
        }

        [HttpDelete("{clanId}")]
        public async Task<IActionResult> DeleteClan(Guid clanId)
        {
            var result = await _clanService.DeleteClanAsync(clanId);
            if (!result)
                return NotFound(new ErrorDto { Message = "Clan not found or already deleted." });

            return Ok();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetClansByUserIdAsync(string userId)
        {
            List<Clan> clans = await _clanService.GetClansByUserIdAsync(userId);
            var clanReadDtos = _mapper.Map<List<ClanReadDto>>(clans);
            return Ok(clanReadDtos);
        }
    }
}