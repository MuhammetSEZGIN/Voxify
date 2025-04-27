using System;
using IdentityService.Models;

namespace IdentityService.DTOs;

public class RegisterResult
{
    public bool Succeeded { get; set; }
    public ApplicationUser ApplicationUser {get; set;}
    public string ErrorMessage { get; set; }

    private RegisterResult() { }   

    public static RegisterResult Success(ApplicationUser userDto)
    {
        return new RegisterResult
        {
            Succeeded = true,
            ApplicationUser = userDto
        };
    }
    public static RegisterResult Failure(string errorMessage)
    {
        return new RegisterResult
        {
            Succeeded = false,
            ErrorMessage = errorMessage
        };
    }
}
