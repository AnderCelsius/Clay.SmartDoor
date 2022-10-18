using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Infrastructure.Data;
using Clay.SmartDoor.Test.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using WebMotions.Fake.Authentication.JwtBearer;

namespace Clay.SmartDoor.Test.Infrastructure
{
    internal class SmartDoorApplication<T> : WebApplicationFactory<T> where T : class
    {
        private SqliteConnection? _connection;

        // Default logged in user for all requests - can be overwritten in individual tests
        private readonly MockAuthUser _user = new MockAuthUser(
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim("email", "default-user@xyz.com"));

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<SmartDoorContext>));

                services.AddTestAuthentication();
                services.AddScoped(_ => _user);

                services.AddDbContext<SmartDoorContext>(options =>
                {
                    options
                        .UseSqlite(_connection)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();

                    var fac = LoggerFactory.Create(b => _ = b.AddDebug());
                    options.UseLoggerFactory(fac);

                    // Build the service provider.
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<SmartDoorContext>();

                    var userManager = sp.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

                    SmartDoorDataSeeder.SeedAsync(db, userManager, roleManager).GetAwaiter().GetResult();
                });
            })
            .ConfigureLogging(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

        }

        protected override void Dispose(bool disposing)
        {
            _connection?.Dispose();
            base.Dispose(disposing);
        }
    }
}
