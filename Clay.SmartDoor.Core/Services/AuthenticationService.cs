using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Clay.SmartDoor.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            ILogger logger,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<ApiResponse<LoginResponse>> AuthenticateUserAsync(LoginRequest loginRequest)
        {
            try
            {
                _logger.Information("Attempting Login...");
                var (succeeded, message, statusCode, user) = await ValidateUser(loginRequest);

                if (!succeeded)
                {
                    _logger.Information(message);
                    return ApiResponse<LoginResponse>
                        .Fail(message, statusCode);
                }

                var result = new LoginResponse
                {
                    UserId = user.Id,
                    Token = await GenerateToken(user)
                };

                return ApiResponse<LoginResponse>.Success(message, result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<LoginResponse>.Fail(Constants.Generic_Failure_Message);
            }
        }

        /// <summary>
        /// Validates a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns true if the user exists</returns>
        private async Task<(bool succeeded, string error, int statusCode, AppUser data)> ValidateUser(LoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return (false, AuthenticationMessage.Invalid_Credentials, (int)HttpStatusCode.Unauthorized, null!);
            }
            if (!await _userManager.IsEmailConfirmedAsync(user) || !user.IsActive)
            {
                return (false, AuthenticationMessage.Not_Activated, (int)HttpStatusCode.Forbidden, null!);
            }
            return (true, AuthenticationMessage.Login_Success, 200, user);
        }

        /// <summary>
        /// Generates a bearer JWT token for a logged user which is used for Authorization
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GenerateToken(AppUser user)
        {

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
            };

            var secret = _configuration.GetSection("Jwt").GetSection("key").Value;

            var key = Encoding.UTF8.GetBytes(secret);

            //Gets the roles of the logged in user and adds it to Claims
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = new List<Claim>();

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));

                var identityRole = await _roleManager.FindByNameAsync(role);
                if(identityRole != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(identityRole);
                    permissions.AddRange(roleClaims.Where(rc => rc.Type == "Permission").ToList());
                }
            }

            authClaims.AddRange(permissions);

            var signingKey = new SymmetricSecurityKey(key);

            var token = new JwtSecurityToken
            (audience: _configuration["Jwt:Audience"],
             issuer: _configuration["Jwt:Issuer"],
             claims: authClaims,
             expires: DateTime.Now.AddMinutes(20),
             signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
