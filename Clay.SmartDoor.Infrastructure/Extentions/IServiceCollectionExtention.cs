using Clay.SmartDoor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clay.SmartDoor.Infrastructure.Extentions
{
    public static class IServiceCollectionExtention
    {
        public static void ConfigureDBContext(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
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
        }
    }
}
