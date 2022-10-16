using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Helpers;
using Clay.SmartDoor.Core.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Clay.SmartDoor.Infrastructure.Data
{
    public class SeedData
    {
        private const string Default_Password = "Password@123";
        public static async Task SeedDefaultRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
        }

        private static AppUser SuperAdminUser = new()
        {
            FirstName = "Obinna",
            LastName = "Asiegbulam",
            Email = "SuperAdminUser@Email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
        };

        private static AppUser AdminUser = new()
        {
            FirstName = "Joshua",
            LastName = "Enyi",
            Email = "AdminUser@Email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };

        private static AppUser BasicUser = new()
        {
            FirstName = "Omowunmi",
            LastName = "Kassim",
            Email = "BasicUser@Email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };
    }
}
