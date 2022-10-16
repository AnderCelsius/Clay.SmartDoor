using Clay.SmartDoor.Core.Entities;
using Microsoft.AspNetCore.Identity;

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

            }
        }
    }
}
