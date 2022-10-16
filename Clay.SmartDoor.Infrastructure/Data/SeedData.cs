using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Helpers;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Clay.SmartDoor.Infrastructure.Data
{
    public static class SeedData
    {
        private const string Default_Password = "Password@123";
        public static async Task SeedDefaultRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
        }
        public static async Task SeedSuperAdminUsersAsync(
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            var user = await userManager.FindByEmailAsync(SuperAdminUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(SuperAdminUser, Default_Password);

                await userManager.AddToRoleAsync(SuperAdminUser, Roles.SuperAdmin.ToString());
                await userManager.AddToRoleAsync(AdminUser, Roles.Admin.ToString());
                await userManager.AddToRoleAsync(AdminUser, Roles.Basic.ToString());
            }

            await roleManager.SeedClaimsForSuperAdmin();
        }

        public static async Task SeedAdminUsersAsync(
            UserManager<AppUser> userManager)
        {
            var user = await userManager.FindByEmailAsync(AdminUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(AdminUser, Default_Password);

                await userManager.AddToRoleAsync(AdminUser, Roles.Admin.ToString());
                await userManager.AddToRoleAsync(AdminUser, Roles.Basic.ToString());
            }
        }

        public static async Task SeedBasicUsersAsync(
            UserManager<AppUser> userManager)
        {

            var user = await userManager.FindByEmailAsync(BasicUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(BasicUser, Default_Password);

                await userManager.AddToRoleAsync(BasicUser, Roles.Basic.ToString());
            }
        }

        private async static Task SeedClaimsForSuperAdmin(
            this RoleManager<IdentityRole> roleManager)
        {
            var superAdminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
            await roleManager.AddPermissionClaim(superAdminRole, "Door");
        }

        private async static Task AddPermissionClaim(
            this RoleManager<IdentityRole> roleManager, 
            IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissions(module);

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", permission));
                }

            }

        }

        #region User Objects

        private static AppUser SuperAdminUser = new()
        {
            FirstName = "Obinna",
            LastName = "Asiegbulam",
            Email = "SuperAdminUser@Email.com",
            UserName = "SuperAdminUser@Email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
        };

        private static AppUser AdminUser = new()
        {
            FirstName = "Joshua",
            LastName = "Enyi",
            Email = "AdminUser@Email.com",
            UserName = "AdminUser@Email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive=true
        };

        private static AppUser BasicUser = new()
        {
            FirstName = "Omowunmi",
            LastName = "Kassim",
            Email = "BasicUser@Email.com",
            UserName = "BasicUser@Email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true
        };
        #endregion
    }
}
