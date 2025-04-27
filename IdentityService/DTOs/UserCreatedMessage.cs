using System;
using System.Runtime.Serialization;

namespace IdentityService.Models;

public class UserCreatedMessage
{
    public string UserName { get; set; }
    public string Email { get; set; }
   
}
