using System;
using IdentityService.Interfaces;

namespace IdentityService.Services;

public class IpAddressService : IIpAddressService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IpAddressService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetClientIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            return "Unknown";
        }
        return context.Connection.RemoteIpAddress?.ToString();
    }
}
