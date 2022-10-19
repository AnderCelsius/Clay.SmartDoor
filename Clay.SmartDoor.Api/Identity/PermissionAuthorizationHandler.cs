using Clay.SmartDoor.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Clay.SmartDoor.Api.Identity;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly SmartDoorContext _smartDoorContext;

    public PermissionAuthorizationHandler(SmartDoorContext smartAcContext)
    {
        _smartDoorContext = smartAcContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if(context.User == null)
        {
            return;
        }

        var permissions = context.User.Claims.Where( x => x.Type == "Permission" &&
                                x.Value == requirement.Permission &&
                                x.Issuer == "https://localhost:7114");

        if (permissions.Any())
        {
            context.Succeed(requirement);
        }

        return;
    }
}
