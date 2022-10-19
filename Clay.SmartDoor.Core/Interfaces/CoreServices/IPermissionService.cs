using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Models;

namespace Clay.SmartDoor.Core.Interfaces.CoreServices
{
    public interface IPermissionService
    {
        Task<ApiResponse<RolePermissionsDto>> GetAsync(string roleId);
        Task<ApiResponse<UserPermissions>> GetUserPermissionsAsync(string userId);
        Task<ApiResponse<string>> UpdateUserPermissionsAsync(UserPermissions permissionsDto);
        Task<ApiResponse<string>> UpdatePermissionsForRoleAsync(RolePermissionsDto permissionsDto);
    }
}
