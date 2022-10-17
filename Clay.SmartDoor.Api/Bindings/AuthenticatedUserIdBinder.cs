using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Clay.SmartDoor.Api.Bindings
{
    /// <summary>
    /// Retrieves the Indentity Name of the user making therequest 
    /// and bindds it to the request userId
    /// </summary>
    public class AuthenticatedUserIdBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.FieldName == "userId")
            {
                var userId = bindingContext.ActionContext.HttpContext.User.Identity?.Name ?? string.Empty;
                bindingContext.Result = ModelBindingResult.Success(userId);
            }

            return Task.CompletedTask;
        }
    }
}
