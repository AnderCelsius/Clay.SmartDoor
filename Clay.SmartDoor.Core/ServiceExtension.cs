using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Clay.SmartDoor.Core
{
    public static class ServiceExtension
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Add Dependencies
            services.AddScoped<IDoorService, DoorService>();
            services.AddScoped<IActivityLogService, ActivityLogService>();
        }
    }
}
