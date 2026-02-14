using System;
using System.ComponentModel.DataAnnotations;
using IdentityService.Attributes;

namespace IdentityService.DTOs;

public class RegisterModel
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(
        100,
        MinimumLength = 3,
        ErrorMessage = "Username cannot be longer than 100 characters."
    )]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [PasswordValidation]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string PasswordConfirmation { get; set; }
    public string DeviceInfo { get; set; }
    public string AvatarUrl { get; set; }
}
