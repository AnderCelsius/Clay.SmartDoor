using Clay.SmartDoor.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Clay.SmartDoor.Api.Identity;

public class ValidTokenAuthorizationHandler : AuthorizationHandler<SpecialDoorAccessRequirement>
{
    private readonly SmartDoorContext _smartDoorContext;

    public ValidTokenAuthorizationHandler(SmartDoorContext smartAcContext)
    {
        _smartDoorContext = smartAcContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        SpecialDoorAccessRequirement requirement)
    {
        var tokenId = context.User.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
        var deviceSerialNumber = context.User.Identity?.Name;

        
    }
}
