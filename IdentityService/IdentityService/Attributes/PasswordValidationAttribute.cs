using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IdentityService.Attributes;

public class PasswordValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value == null)
            return false;
        string password = value.ToString();
        var errors = new List<string>();
        if (password.Length < 6)
        {
            errors.Add("Password must be at least 6 characters long.");
        }

        // En az bir büyük harf
        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            errors.Add("Password must contain at least one uppercase letter.");
        }

        // En az bir küçük harf
        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            errors.Add("Password must contain at least one lowercase letter.");
        }

        // En az bir rakam
        if (!Regex.IsMatch(password, @"\d"))
        {
            errors.Add("Password must contain at least one digit.");
        }

        // En az bir özel karakter
        if (!Regex.IsMatch(password, @"[@$!%*?&]"))
        {
            errors.Add("Password must contain at least one special character (@$!%*?&).");
        }

        if (errors.Count > 0)
        {
            ErrorMessage = string.Join(";", errors);
            return false;
        }
        return true;
    }
}
