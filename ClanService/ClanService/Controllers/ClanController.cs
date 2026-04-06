using Microsoft.AspNetCore.Mvc;
using ClanService.Interfaces;
using ClanService.Models;
using ClanService.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var (created, message) = await _clanService.CreateClanAsync(clan, userId);
            if (created == null)
                return BadRequest(new ErrorDto { Message = message });
            var readDto = _mapper.Map<ClanReadDto>(created);
            return Ok(readDto);
        }

        [HttpGet("clanId/{clanId}")]
        [Authorize(Roles = "OWNER,ADMIN,MEMBER")]
        public async Task<IActionResult> GetClanById(Guid clanId)
        {
            var clan = await _clanService.GetClanByIdAsync(clanId);
            if (clan == null)
                return NotFound(new ErrorDto { Message = "Clan not found." });

            var readDto = _mapper.Map<GetAllClanPropertyDto>(clan);
            return Ok(readDto);
        }

        [HttpGet]
        [Authorize(Roles = "MUHAMMET")]
        public async Task<IActionResult> GetAllClans()
        {
            var clans = await _clanService.GetAllClansAsync();
            var readDtoList = _mapper.Map<List<ClanReadDto>>(clans);
            return Ok(readDtoList);
        }

        [HttpPut("clanId/{clanId}")]
        [Authorize(Roles = "OWNER,ADMIN")]
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
            existing.Description = dto.Description;
            var updated = await _clanService.UpdateClanAsync(existing);
            var readDto = _mapper.Map<ClanReadDto>(updated);
            return Ok(readDto);
        }

        [HttpDelete("clanId/{clanId}")]
        [Authorize(Roles = "OWNER")]
        public async Task<IActionResult> DeleteClan(Guid clanId)
        {
            var result = await _clanService.DeleteClanAsync(clanId);
            if (!result)
                return NotFound(new ErrorDto { Message = "Clan not found or already deleted." });

            return Ok();
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetMyClansAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new ErrorDto { Message = "User ID not found in request headers." });

            List<Clan> clans = await _clanService.GetClansByUserIdAsync(userId);
            var clanReadDtos = _mapper.Map<List<ClanReadDto>>(clans);
            return Ok(clanReadDtos);
        }
    }
}