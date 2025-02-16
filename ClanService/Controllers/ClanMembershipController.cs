using Microsoft.AspNetCore.Mvc;
using ClanService.Interfaces;
using ClanService.Models;
using ClanService.DTOs;
using AutoMapper;
using ClanService.DTOs.ClanDtos;

namespace ClanService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClanMembershipController : ControllerBase
    {
        private readonly IClanMembershipService _clanMembershipService;
        private readonly IMapper _mapper;   

        public ClanMembershipController(IClanMembershipService clanMembershipService, IMapper mapper)
        {
            _clanMembershipService = clanMembershipService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> AddMember([FromBody] ClanMembershipCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto 
                { 
                    Message = "Invalid membership data.", 
                    Errors = ModelState 
                });

            var membership = _mapper.Map<ClanMembership>(dto);
            membership.Role= ClanRole.Member;
            var created = await _clanMembershipService.AddMemberAsync(membership);
            var readDto = _mapper.Map<ClanMembershipReadDto>(created);  

            return Ok(readDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMembership(Guid id)
        {
            var membership = await _clanMembershipService.GetMembershipAsync(id);
            if (membership == null)
                return NotFound(new ErrorDto { Message = "Membership not found." });

            var readDto = _mapper.Map<ClanMembershipReadDto>(membership);
            return Ok(readDto);
        }

        [HttpGet("clan/{clanId}")]
        public async Task<IActionResult> GetMembershipsByClanId(Guid clanId)
        {
            var memberships = await _clanMembershipService.GetMembershipsByClanIdAsync(clanId);
            var readDtoList = _mapper.Map<List<ClanMembershipReadDto>>(memberships);
            return Ok(readDtoList);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetMembershipsByUserId(string userId)
        {
            var memberships = await _clanMembershipService.GetMembershipsByUserIdAsync(userId);
            var readDtoList = _mapper.Map<List<ClanMembershipReadDto>>(memberships);    
            return Ok(readDtoList);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveMember(Guid id)
        {
            var result = await _clanMembershipService.RemoveMemberAsync(id);
            if (!result)
                return NotFound(new ErrorDto { Message = "Membership not found or already removed." });

            return NoContent();
        }
    }
}