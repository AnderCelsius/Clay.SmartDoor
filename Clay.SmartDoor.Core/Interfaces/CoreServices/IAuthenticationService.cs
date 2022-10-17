using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Models;

namespace Clay.SmartDoor.Core.Interfaces.CoreServices
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns>Returns a response depending on whether the users credentials satisfies the system requirements.</returns>
        Task<ApiResponse<LoginResponse>> AuthenticateUserAsync(LoginRequest loginRequest);
    }
}
