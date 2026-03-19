using Microsoft.AspNetCore.Mvc.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // 1. Get the instance (assuming it's the first argument or known by name)
        var model = context.ActionArguments.Values.FirstOrDefault();

        if (model != null)
        {
            // 2. Get the type
            Type modelType = model.GetType();

            Console.WriteLine($"Validating instance of: {modelType.Name}");
        }

        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}
