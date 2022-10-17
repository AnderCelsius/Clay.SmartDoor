using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clay.SmartDoor.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(
            IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PermissionsDto>>> GetPermissionsByRole(string roleId)
        {
            var result = await _permissionService.GetAsync(roleId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("PermissionsForUser")]
        public async Task<ActionResult<ApiResponse<PermissionsDto>>> GetPermissionsByUser(string userId)
        {
            var result = await _permissionService.GetUserPermissionsAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> UpdatePermissionsForRole(
            [FromBody] PermissionsDto payload)
        {
            var result = await _permissionService.UpdatePermissionsForRoleAsync(payload);
            return StatusCode(result.StatusCode, result);
        }
    }
}
