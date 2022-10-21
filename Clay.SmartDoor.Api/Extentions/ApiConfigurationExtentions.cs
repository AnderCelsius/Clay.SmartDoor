using Clay.SmartDoor.Api.Identity;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Clay.SmartDoor.Api.Extentions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApiConfigurationExtentions
    {
        /// <summary>
        /// Adds and configures the identity system for <seealso cref="AppUser"/>
        /// and <seealso cref="IdentityRole"/>. Then uses the IdentityBuilder to 
        /// add EnityFramework implementation for identityb information stores.
        /// </summary>
        /// <param name="services"></param>
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

        /// <summary>
        /// Registers custom permission policy provider.
        /// </summary>
        /// <param name="services"></param>
        public static void AddSmartDoorPermissionPolicy(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }

        /// <summary>
        /// Configures OpenApi documentationfor the project.
        /// </summary>
        /// <param name="services"></param>
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
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                c.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// Configures JWT services and adds Authorization with the necessary policy.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var signingKey = configuration["Jwt:Key"];

            services.AddAuthorization();

            services
                .AddAuthentication(options =>
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
                options.AddPolicy("Access.Read", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("Permission", Permissions.Access.Read);
                });
                options.AddPolicy("Access.Create", policy =>
                 {
                     policy.RequireAuthenticatedUser();
                     policy.RequireClaim("Permission", Permissions.Access.Create);
                 });
                options.AddPolicy("Access.Grant", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("Permission", Permissions.Access.Grant);
                });
                options.AddPolicy("Access.Revoke", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("Permission", Permissions.Access.Revoke);
                });
                options.AddPolicy("User.Create",
                    policy => policy.RequireClaim("Permission", Permissions.User.Create));
            });

            services.AddHttpContextAccessor();
        }

        /// <summary>
        /// Regiters the SwaggerUI middleware
        /// </summary>
        /// <param name="app"></param>
        public static void UseOpenApiDocumentation(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartDoor API V1"); });
        }

        /// <summary>
        /// Adds enpoint to map controleer actions.
        /// </summary>
        /// <param name="app"></param>
        public static void MapSmartDoorControllers(this WebApplication app)
        {
            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
        }


        /// <summary>
        /// Seeds necessary data if database is empty.
        /// </summary>
        /// <param name="app"></param>
        public static void EnsureDatabaseSetup(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var db = services.GetRequiredService<SmartDoorContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            SmartDoorDataSeeder.SeedAsync(db, userManager, roleManager).GetAwaiter().GetResult();
        }

        /// <summary>
        /// registers <seealso cref="Serilog"/> for the project
        /// </summary>
        /// <param name="services"></param>
        public static void AddSeriLog(this IServiceCollection services)
        {
            services.AddSingleton(Log.Logger);
        }

        public static void AddSmartDoorCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("SamrtDoorpolicy",
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:7114",
                                                  "https://localhost:7114")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod();
                          });
            });
        }
        
    }
}
