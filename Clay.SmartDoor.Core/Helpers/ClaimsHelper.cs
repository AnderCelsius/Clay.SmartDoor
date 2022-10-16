using Clay.SmartDoor.Core.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Security.Claims;

namespace Clay.SmartDoor.Core.Helpers
{
    public static class ClaimsHelper
    {
        public static void GetPermission(this List<RoleClaimDto> allPermissions,
            Type policy)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (var item in fields)
            {
                allPermissions.Add(new RoleClaimDto() { Value = item.GetValue(null).ToString(), Type = "Permissions" });
            }
        }

        public async static Task AddPermissionClaim(
            this RoleManager<IdentityRole> roleManager, 
            IdentityRole role, string permission)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);

            if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }
    }
}
