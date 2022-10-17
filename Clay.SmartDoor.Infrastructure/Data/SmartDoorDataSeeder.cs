using Clay.SmartDoor.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Clay.SmartDoor.Infrastructure.Data
{
    public class SmartDoorDataSeeder
    {
        public static async Task SeedAsync(
            SmartDoorContext context,
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            await context.Database.EnsureCreatedAsync();

            if (!context.Users.Any())
            {
                Log.Information("Preparing to seed data...");

                await SeedData.SeedAccessGroups(context);
                await SeedData.SeedDefaultRolesAsync(roleManager);
                await SeedData.SeedBasicUsersAsync(userManager);
                await SeedData.SeedAdminUsersAsync(userManager);
                await SeedData.SeedSuperAdminUsersAsync(userManager, roleManager);

                Log.Information("Seeding Completed.");
            }
        }
    }
}
