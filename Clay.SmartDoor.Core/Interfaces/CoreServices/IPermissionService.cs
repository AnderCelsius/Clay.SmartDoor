using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clay.SmartDoor.Core.Interfaces.CoreServices
{
    public interface IPermissionService
    {
        Task<ApiResponse<PermissionsDto>> GetAsync(string roleId);
        Task<ApiResponse<PermissionsDto>> GetUserPermissionsAsync(string userId);
        Task<ApiResponse<string>> UpdatePermissionsForRoleAsync(PermissionsDto permissionsDto);
    }
}
