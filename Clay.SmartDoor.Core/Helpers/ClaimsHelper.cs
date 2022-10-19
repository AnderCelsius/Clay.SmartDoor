using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Security.Claims;

namespace Clay.SmartDoor.Core.Helpers
{
    public static class ClaimsHelper
    {
        public static void GetPermission(this List<ClaimDto> allPermissions,
            Type policy)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (var item in fields)
            {
                allPermissions.Add(new ClaimDto() { Value = item.GetValue(null).ToString(), Type = "Permissions" });
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

        public async static Task AddPermissionClaim(
            this UserManager<AppUser> userManager,
            AppUser user, string permission)
        {
            var allClaims = await userManager.GetClaimsAsync(user);

            if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
            {
                await userManager.AddClaimAsync(user, new Claim("Permission", permission));
            }
        }
    }
}
