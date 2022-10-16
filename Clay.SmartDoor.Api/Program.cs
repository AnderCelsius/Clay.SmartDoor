using Clay.SmartDoor.Api.Extentions;
using Clay.SmartDoor.Infrastructure.Extentions;
using Serilog;
using System.Text.Json.Serialization;

// Add Serilog setup
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var isDevelopment = environment == Environments.Development;

IConfiguration config = ApiConfigurationExtentions.GetConfig(isDevelopment);
LogSettings.SetUpSerilog(config);

try
{
    Log.Information("Application is starting...");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers()
            .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

    builder.Services.AddSmartDoorServices();
    builder.Services.AddInfrastructureServices(config);
    builder.Services.AddOpenApiDocumentation();


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.EnsureDatabaseSetup();
    }

    app.UseHttpsRedirection();

    app.UseOpenApiDocumentation();
    app.UseAuthorization();

    app.MapSmartDoorControllers();
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e.Message, e.StackTrace, "The application failed to start");
}
finally
{
    Log.CloseAndFlush();
}

