using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models;

public class LoginRequestModel
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; }
    public string DeviceInfo { get; set; }
}
