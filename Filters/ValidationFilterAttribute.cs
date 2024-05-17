using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TZTDate_UserWebApi.Filters;

public class ValidationFilterAttribute : IActionFilter
{
  public void OnActionExecuted(ActionExecutedContext context) { }

  public void OnActionExecuting(ActionExecutingContext context)
  {
    if (context.ModelState.IsValid == false)
    {
      context.Result = new UnprocessableEntityObjectResult(context.ModelState);
    }
  }
}
