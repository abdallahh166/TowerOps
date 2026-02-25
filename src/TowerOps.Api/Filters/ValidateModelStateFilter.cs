namespace TowerOps.Api.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using TowerOps.Api.Errors;
using TowerOps.Api.Localization;

public class ValidateModelStateFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>());

        var localizer = context.HttpContext.RequestServices.GetService<ILocalizedTextService>() ?? new LocalizedTextService();
        var mapped = ApiErrorFactory.Validation(
            errors,
            localizer,
            context.HttpContext.TraceIdentifier);

        context.Result = new ObjectResult(mapped.Error)
        {
            StatusCode = mapped.StatusCode
        };
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No-op
    }
}

