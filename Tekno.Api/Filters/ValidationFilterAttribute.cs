
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tekno.Api.Commons.Responses;

public class ValidationFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(kv => kv.Value.Errors.Count > 0)
                .ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var result = ApiResponse<object>.Fail("Validation failed", errors);
            context.Result = new BadRequestObjectResult(result);
        }
    }
}
