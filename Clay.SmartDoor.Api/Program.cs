using Clay.SmartDoor.Api.Extentions;
using Clay.SmartDoor.Core;
using Clay.SmartDoor.Infrastructure;
using Serilog;

// Add Serilog setup
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var isDevelopment = environment == Environments.Development;

IConfiguration config;

try
{
    Log.Information("Application is starting...");

    var builder = WebApplication.CreateBuilder(args);
    config = builder.Configuration;
    LogSettings.SetUpSerilog(builder.Configuration);


    // Add services to the container.

    builder.Services.AddControllers()
        .AddNewtonsoftJson(op => op.SerializerSettings
                    .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

    builder.Services.AddSeriLog();
    builder.Services.AddSmartDoorCors();
    builder.Services.AddSmartDoorPermissionPolicy();
    builder.Services.AddInfrastructureServices(config);
    builder.Services.AddOpenApiDocumentation();

    builder.Services.AddSmartDoorIdentity();
    builder.Services.AddJwtAuthentication(config);
    builder.Services.AddCoreServices();


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.EnsureDatabaseSetup();
    }

    app.UseHttpsRedirection();

    app.UseOpenApiDocumentation();
    app.UseCors("SamrtDoorpolicy");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapSmartDoorControllers();
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, e.Message, "The application failed to start");
}
finally
{
    Log.CloseAndFlush();
}

