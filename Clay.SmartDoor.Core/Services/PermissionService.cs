using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Helpers;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Constants;
using Microsoft.AspNetCore.Identity;

namespace Clay.SmartDoor.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public PermissionService(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<ApiResponse<PermissionsDto>> GetAsync(string roleId)
        {
            var allPermissions = new List<RoleClaimDto>();

            allPermissions.GetPermission(typeof(Permissions.Door));

            var role = await _roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                return ApiResponse<PermissionsDto>.Fail("Some");
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            var allClaimValues = allPermissions.Select(p => p.Value).ToList();
            var roleClaimValues = claims.Select(c => c.Value).ToList();
            var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();

            foreach(var permission in allPermissions)
            {
                if(authorizedClaims.Any(ac => ac == permission.Value))
                {
                    permission.Selected = true;
                }
            }

            return new ApiResponse<PermissionsDto>()
            {
                Data = new PermissionsDto
                {
                    RoleId = roleId,
                    RoleClaims = allPermissions
                },
                StatusCode = 200,
                Succeeded = true
            };
        }

        public async Task<ApiResponse<PermissionsDto>> GetUserPermissionsAsync(string userId)
        {
            var allPermissions = new List<RoleClaimDto>();

            allPermissions.GetPermission(typeof(Permissions.Door));
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return ApiResponse<PermissionsDto>.Fail("Some");
            }

            var claims = await _userManager.GetClaimsAsync(user);

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

            return new ApiResponse<PermissionsDto>()
            {
                Data = new PermissionsDto
                {
                    RoleId = userId,
                    RoleClaims = allPermissions
                },
                StatusCode = 200,
                Succeeded = true
            };
        }

        public async Task<ApiResponse<string>> UpdatePermissionsForRoleAsync(PermissionsDto model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
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
            catch (Exception)
            {

                return ApiResponse<string>.Fail(Constants.Generic_Failure_Message);
            }
        }
    }
}
