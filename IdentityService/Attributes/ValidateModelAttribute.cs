using System;
using IdentityService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityService.Attributes;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = string.Join(
                "; ",
                context
                    .ModelState.Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
            );

            var result = ApiResponse<object>.Failed(errors);
            context.Result = new ObjectResult(result) { StatusCode = 400 };
            Console.WriteLine($"Model validation failed: {errors}");
        }
    }
}
