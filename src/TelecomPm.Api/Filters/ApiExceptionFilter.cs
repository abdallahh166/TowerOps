namespace TelecomPM.Api.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TelecomPM.Domain.Exceptions;

public class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ValidationException validationException)
        {
            return;
        }

        context.Result = new BadRequestObjectResult(new
        {
            Message = "Validation failed",
            Errors = validationException.Errors
        });

        context.ExceptionHandled = true;
    }
}

