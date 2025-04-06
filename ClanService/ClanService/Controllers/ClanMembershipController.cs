using Microsoft.AspNetCore.Mvc;
using ClanService.Interfaces;
using ClanService.Models;
using ClanService.DTOs;
using AutoMapper;
using ClanService.DTOs.ClanDtos;
using ClanService.DTOs.ClanMembershipDtos;

namespace ClanService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClanMembershipController : ControllerBase
    {
        private readonly IClanMembershipService _clanMembershipService;
        private readonly IClanService _clanService;
        private readonly IMapper _mapper;

        public ClanMembershipController(IClanMembershipService clanMembershipService, IMapper mapper, IClanService clanService)
        {
            _clanMembershipService = clanMembershipService;
            _mapper = mapper;
            _clanService = clanService;
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
        
        [HttpDelete("{clanId}/user/{userId}")]
        public async Task<IActionResult> LeaveClan(string userId, Guid clanId)
        {
            var result = await _clanMembershipService.LeaveClanAsync(userId, clanId);
            if (result.Item1.Equals(null))
                return NotFound(new ErrorDto { Message =result.Item2 });

            return NoContent();
        }
        [HttpPost("{clanId}/invitations")]
        public async Task<IActionResult> CreateInvitation(Guid clanId)
        {
            var clan = await _clanService.GetClanByIdAsync(clanId);
            if (clan == null)
                return NotFound(new ErrorDto { Message = "Clan not found." });

            var invitation = await _clanService.CreateInviteTokenAsync(clanId);

            return Ok(new
            {
                InviteCode = invitation.InviteCode,
                ExpiresAt = invitation.ExpiresAt,
                MaxUses = invitation.MaxUses
            });
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinClanWithInvite([FromBody] InviteCodeDto inviteCode)
        {
            var (isValid,validateMessage, invitation) = await _clanService.ValidateAndUseInvitationAsync(inviteCode.InviteCode);
            if (!isValid)
                return BadRequest(new ErrorDto { Message = validateMessage });

            var membership = new ClanMembership
            {
                ClanId = invitation.ClanId,
                UserId = inviteCode.UserId,
                Role = ClanRole.Member
            };

            var (result, message) = await _clanMembershipService.AddMemberAsync(membership);
            if(result == null)
                return BadRequest(new ErrorDto { Message = message });
            
            return Ok(_mapper.Map<ClanMembershipReadDto>(result));
        }
    }
}