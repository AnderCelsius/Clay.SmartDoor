using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Clay.SmartDoor.Api.Bindings
{
    public class AppUserIdBinder : IModelBinder
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
