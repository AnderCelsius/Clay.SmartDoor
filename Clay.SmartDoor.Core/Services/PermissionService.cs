using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Helpers;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Constants;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Clay.SmartDoor.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger _logger;


        public PermissionService(
            ILogger logger,
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }
       
        public async Task<ApiResponse<RolePermissionsDto>> GetAsync(string roleId)
        {
            try
            {
                var allPermissions = new List<ClaimDto>();
                var accessPermissions = new List<ClaimDto>();
                var userPermissions = new List<ClaimDto>();

                accessPermissions.GetPermission(typeof(Permissions.Access));
                userPermissions.GetPermission(typeof(Permissions.User));

                allPermissions.AddRange(accessPermissions);
                allPermissions.AddRange(userPermissions);

                var role = await _roleManager.FindByIdAsync(roleId);

                if (role == null)
                {
                    return ApiResponse<RolePermissionsDto>.Fail(Constants.Generic_Not_Found_Message);
                }

                var claims = await _roleManager.GetClaimsAsync(role);

                var allClaimValues = allPermissions.Select(p => p.Value).ToList();
                var roleClaimValues = claims.Select(c => c.Value).ToList();
                var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();

                foreach (var permission in allPermissions)
                {
                    if (authorizedClaims.Any(ac => ac == permission.Value))
                    {
                        permission.Selected = true;
                    }
                }

                return new ApiResponse<RolePermissionsDto>()
                {
                    Data = new RolePermissionsDto
                    {
                        RoleId = roleId,
                        RoleClaims = allPermissions
                    },
                    StatusCode = 200,
                    Succeeded = true
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<RolePermissionsDto>.Fail(Constants.Generic_Operation_Failed_Message, 400);
            }
        }

        public async Task<ApiResponse<UserPermissions>> GetUserPermissionsAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return ApiResponse<UserPermissions>.Fail(Constants.Generic_Not_Found_Message);
                }

                var allPermissions = new List<ClaimDto>();
                var accessPermissions = new List<ClaimDto>();
                var userPermissions = new List<ClaimDto>();

                accessPermissions.GetPermission(typeof(Permissions.Access));
                userPermissions.GetPermission(typeof(Permissions.User));

                allPermissions.AddRange(accessPermissions);
                allPermissions.AddRange(userPermissions);              

                var claims = await _userManager.GetClaimsAsync(user);

                var allClaimValues = allPermissions.Select(p => p.Value).ToList();
                var userClaimValues = claims.Select(c => c.Value).ToList();
                var authorizedClaims = allClaimValues.Intersect(userClaimValues).ToList();

                foreach (var permission in allPermissions)
                {
                    if (authorizedClaims.Any(ac => ac == permission.Value))
                    {
                        permission.Selected = true;
                    }
                }

                return new ApiResponse<UserPermissions>()
                {
                    Data = new UserPermissions
                    {
                        UserId = userId,
                        Claims = allPermissions
                    },
                    StatusCode = 200,
                    Succeeded = true
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<UserPermissions>.Fail(Constants.Generic_Failure_Message, 400);
            }
        }

        public async Task<ApiResponse<string>> UpdatePermissionsForRoleAsync(RolePermissionsDto model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);

                if(role == null)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Not_Found_Message);
                }

                var claims = await _roleManager.GetClaimsAsync(role);

                foreach (var claim in claims)
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }

                var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();

                foreach (var claim in selectedClaims)
                {
                    await _roleManager.AddPermissionClaim(role, claim.Value);
                }

                var updateClaims = await _roleManager.GetClaimsAsync(role);

                return ApiResponse<string>.Success(Constants.Generic_Success_Message, model.RoleId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Failure_Message, 400);
            }
        }

        public async Task<ApiResponse<string>> UpdateUserPermissionsAsync(UserPermissions model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Not_Found_Message);
                }

                var claims = await _userManager.GetClaimsAsync(user);

                foreach (var claim in claims)
                {
                    await _userManager.RemoveClaimAsync(user, claim);
                }

                var selectedClaims = model.Claims.Where(a => a.Selected).ToList();

                foreach (var claim in selectedClaims)
                {
                    await _userManager.AddPermissionClaim(user, claim.Value);
                }

                var updateClaims = await _userManager.GetClaimsAsync(user);

                return ApiResponse<string>.Success(Constants.Generic_Success_Message, model.UserId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Failure_Message, 400);
            }
        }
    }
}
