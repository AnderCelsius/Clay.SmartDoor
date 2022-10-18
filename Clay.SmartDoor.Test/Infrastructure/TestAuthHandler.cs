using Clay.SmartDoor.Test.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Clay.SmartDoor.Test.Infrastructure
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly MockAuthUser _mockAuthUser;
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            MockAuthUser mockAuthUser)
            : base(options, logger, encoder, clock)
        {
            _mockAuthUser = mockAuthUser;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (_mockAuthUser.Claims.Count == 0)
                return Task.FromResult(AuthenticateResult.Fail("Mock auth user not configured."));

            // 2. Create the principal and the ticket
            var identity = new ClaimsIdentity(_mockAuthUser.Claims, TestConstants.Scheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestConstants.Scheme);

            // 3. Authenticate the request
            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
    }
}
