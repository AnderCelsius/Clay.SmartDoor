using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Infrastructure.Data;
using Clay.SmartDoor.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clay.SmartDoor.Infrastructure
{
    public static class ServiceExtention
    {
        public static void ConfigureDBContext(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("LocalConnection");
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
            services.AddDbContext<SmartDoorContext>(options =>
                options.UseMySql(connectionString, serverVersion));
        }

        public static void AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureDBContext(configuration);

            // Add Dependencies
            services.AddScoped<IDoorRepository, DoorRepository>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        }
    }
}
