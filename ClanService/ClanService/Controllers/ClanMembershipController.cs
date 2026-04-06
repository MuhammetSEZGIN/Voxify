using Microsoft.AspNetCore.Mvc;
using ClanService.Interfaces;
using ClanService.Models;
using ClanService.DTOs;
using AutoMapper;
using ClanService.DTOs.ClanDtos;
using ClanService.DTOs.ClanMembershipDtos;
using Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Authorize(Roles = "MUHAMMET")]
        public async Task<IActionResult> GetMembership(Guid id)
        {
            var membership = await _clanMembershipService.GetMembershipAsync(id);
            if (membership == null)
                return NotFound(new ErrorDto { Message = "Membership not found." });

            var readDto = _mapper.Map<ClanMembershipReadDto>(membership);
            return Ok(readDto);
        }

        [HttpGet("clanId/{clanId}")]
        [Authorize(Roles = "OWNER,ADMIN,MEMBER")]
        public async Task<IActionResult> GetMembershipsByClanId(Guid clanId)
        {
            var memberships = await _clanMembershipService.GetMembershipsByClanIdAsync(clanId);
            var readDtoList = _mapper.Map<List<ClanMembershipReadDto>>(memberships);
            return Ok(readDtoList);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "OWNER,ADMIN,MEMBER")]
        public async Task<IActionResult> GetMembershipsByUserId(string userId)
        {
            var memberships = await _clanMembershipService.GetMembershipsByUserIdAsync(userId);
            var readDtoList = _mapper.Map<List<ClanMembershipReadDto>>(memberships);
            return Ok(readDtoList);
        }

        [HttpDelete("member/{userId}/clanId/{clanId}")]
        [Authorize(Roles = "OWNER,ADMIN")]
        public async Task<IActionResult> RemoveMember(string userId, Guid clanId)
        {
            var result = await _clanMembershipService.RemoveMemberAsync(userId, clanId);
            if (!result)
                return NotFound(new ErrorDto { Message = "Membership not found or already removed." });

            return NoContent();
        }

        [HttpDelete("user/clanId/{clanId}")]
        [Authorize(Roles = "ADMIN,MEMBER")]
        public async Task<IActionResult> LeaveClan(Guid clanId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _clanMembershipService.LeaveClanAsync(userId, clanId);
            if (result.Item1 == null)
                return NotFound(new ErrorDto { Message = result.Item2 });

            return NoContent();
        }
        [HttpPost("invitations/clanId/{clanId}")]
        [Authorize(Roles = "OWNER,ADMIN")]
        public async Task<IActionResult> CreateInvitation(Guid clanId)
        {
            var clan = await _clanService.GetClanByIdAsync(clanId);
            if (clan == null)
                return NotFound(new ErrorDto { Message = "Clan not found." });

            var invitation = await _clanService.CreateInviteTokenAsync(clanId);

            return Ok(new InviteClanDto
            {
                InviteCode = invitation.InviteCode,
                ExpiresAt = invitation.ExpiresAt,
                MaxUses = invitation.MaxUses
            });
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinClanWithInvite([FromBody] InviteCodeDto inviteCode)
        {
            var (isValid, validateMessage, invitation) = await _clanService.ValidateAndUseInvitationAsync(inviteCode.InviteCode);
            if (!isValid)
                return BadRequest(new ErrorDto { Message = validateMessage });
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var membership = new ClanMembership
            {
                ClanId = invitation.ClanId,
                UserId = userId,
                Role = ClanRole.MEMBER.ToString()
            };

            var (result, message) = await _clanMembershipService.AddMemberAsync(membership);
            if (result == null)
                return BadRequest(new ErrorDto { Message = message });

            return Ok(_mapper.Map<ClanMembershipReadDto>(result));
        }
    }
}