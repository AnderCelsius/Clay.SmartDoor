using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Helpers;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Clay.SmartDoor.Infrastructure.Data
{
    public static class SeedData
    {
        private const string Default_Password = "Password@123";
        private const string Default_AccessGroup = "33e09d95-60c1-41ed-a2ae-faff5e711078";
        private const string Default_SecureGroup_One = "ba63545f-2c49-4954-983c-bef094a4027a";
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

            try
            {
                var user = await userManager.FindByEmailAsync(BasicUser.Email);

                if (user == null)
                {
                    await userManager.CreateAsync(BasicUser, Default_Password);

                    await userManager.AddToRoleAsync(BasicUser, Roles.Basic.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private async static Task SeedClaimsForSuperAdmin(
            this RoleManager<IdentityRole> roleManager)
        {
            var superAdminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
            await roleManager.AddPermissionClaim(superAdminRole, "User");
        }

        public async static Task SeedAccessGroups(
           this SmartDoorContext context)
        {
            await context.AccessGroups.AddAsync(SecureAccessGroup);
            await context.AccessGroups.AddAsync(OpenAccessGroup);
            await context.SaveChangesAsync();
        }

        private async static Task AddPermissionClaim(
            this RoleManager<IdentityRole> roleManager, 
            IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissions(module);
            var accessPermissions = Permissions.GenerateAccessPermissions();

            allPermissions.AddRange(allPermissions);

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", permission));
                }

            }

        }

        #region AccessGroup Objects
        private static AccessGroup SecureAccessGroup = new()
        {
            Id = Default_SecureGroup_One,
            Name = "Secure Group",
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = "22eef6fa-2843-4516-a410-f7518703499a",
            LastModified = DateTime.Now,
        };

        private static AccessGroup OpenAccessGroup = new()
        {
            Id = Default_AccessGroup,
            Name = "Oepn Group",
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = "22eef6fa-2843-4516-a410-f7518703499a",
            LastModified = DateTime.Now,
        };
        #endregion

        #region User Objects

        private static AppUser SuperAdminUser = new()
        {
            FirstName = "Obinna",
            LastName = "Asiegbulam",
            Email = "superadminuser@email.com",
            UserName = "superadminuser@email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
            AccessGroupId = Default_SecureGroup_One
        };

        private static AppUser AdminUser = new()
        {
            FirstName = "Joshua",
            LastName = "Enyi",
            Email = "adminuser@email.com",
            UserName = "adminuser@email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive=true,
            AccessGroupId = Default_SecureGroup_One
        };

        private static AppUser BasicUser = new()
        {
            FirstName = "Omowunmi",
            LastName = "Kassim",
            Email = "basicuser@email.com",
            UserName = "basicuser@email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
            AccessGroupId = Default_AccessGroup
        };
        #endregion
    }
}
