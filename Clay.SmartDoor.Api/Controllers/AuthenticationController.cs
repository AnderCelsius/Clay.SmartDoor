using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clay.SmartDoor.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(
            IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Authenticate as user and returns a jwt token for subsequent operations.
        /// </summary>
        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AuthenticateAsync([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.AuthenticateUserAsync(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
