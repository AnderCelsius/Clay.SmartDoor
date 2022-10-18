using Clay.SmartDoor.Test.Helper;
using Clay.SmartDoor.Test.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Clay.SmartDoor.Test.Extensions
{
    public static class AuthServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddTestAuthentication(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(TestConstants.Scheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services.AddAuthentication(TestConstants.Scheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestConstants.Scheme, options => { });
        }
    }
}
