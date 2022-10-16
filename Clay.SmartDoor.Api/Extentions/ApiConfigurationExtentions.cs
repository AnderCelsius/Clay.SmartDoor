using Clay.SmartDoor.Api.Identity;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Clay.SmartDoor.Api.Extentions
{
    public static class ApiConfigurationExtentions
    {
        public static IConfiguration GetConfig(bool isDevelopment)
        {
            return isDevelopment ? new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
            :
            new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
        }

        public static void AddSmartDoorIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<SmartDoorContext>()
                .AddDefaultTokenProviders();
        }
        public static void AddSmartDoorPermissionPolicy(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }
        public static void AddOpenApiDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SmartDoor API",
                    Description = @"An API that grants authorized users access to doors
                            and provides historical events data beyong classical tags. ",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Obinna Asiegbulam",
                        Email = "oasiegbulam@gmail.com"
                    }
                });

                c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "BearerAuth"
                        }
                    },
                    Array.Empty<string>()
                }
            });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                c.IncludeXmlComments(xmlFilename);
            });
        }
        public static void AddJwtAuthentication(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var signingKey = configuration["Jwt:Key"];

            services.AddAuthorization();

            services
                .AddAuthentication( options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidIssuer = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Door.All", policy =>
                    policy.RequireClaim("Permission", "Permissions.Door.All")
                );
            });

            services.AddHttpContextAccessor();
        }
        public static void UseOpenApiDocumentation(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartDoor API V1"); });
        }

        public static void MapSmartDoorControllers(this WebApplication app)
        {
            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
        }

        public static void EnsureDatabaseSetup(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var db = services.GetRequiredService<SmartDoorContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            SmartDoorDataSeeder.SeedAsync(db, userManager, roleManager).GetAwaiter().GetResult(); ;
        }

        public static void AddSeriLog(this IServiceCollection services)
        {
            services.AddSingleton(Log.Logger);
        }
    }
}
