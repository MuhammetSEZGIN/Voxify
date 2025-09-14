using System;

namespace IdentityService.Interfaces;

public interface IIpAddressService
{
    string GetClientIpAddress();
}
