using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(
        100,
        MinimumLength = 3,
        ErrorMessage = "Username cannot be longer than 100 characters."
    )]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(
        100,
        MinimumLength = 3,
        ErrorMessage = "Full Name cannot be longer than 100 characters."
    )]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(
        100,
        MinimumLength = 6,
        ErrorMessage = "Password must be at least 6 characters long."
    )]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public string DeviceInfo { get; set; }
    public string AvatarUrl { get; set; }
}
