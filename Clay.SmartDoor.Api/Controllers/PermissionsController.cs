using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clay.SmartDoor.Api.Controllers
{
    [Authorize]
    [Route("api/v1/permissions")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(
            IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Retrieves all permissions for the given role corresponding to the given roleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        /// <response code="200">When the role exists and the data is fetched successfully</response>
        /// <response code="401">When the caller sends an invalid JWT</response>
        /// <response code="403">When the caller does not have the permissions to view resource</response>
        /// <response code="404">When the role does not exist</response>
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("role-perimissions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RolePermissionsDto>>> GetPermissionsByRole(string roleId)
        {
            var result = await _permissionService.GetAsync(roleId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Retrieves all permissions for the given user corresponding to the given userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>        
        /// <response code="200">When the role exists and the data is fetched successfully</response>
        /// <response code="401">When the caller sends an invalid JWT</response>
        /// <response code="403">When the caller does not have the permissions to view resource</response>
        /// <response code="404">When the role does not exist</response>
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("user-permissions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RolePermissionsDto>>> GetPermissionsByUser(string userId)
        {
            var result = await _permissionService.GetUserPermissionsAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Updates all permissions for claims marked as true of the role corresponding to the given roleId
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <response code="200">When the role exists and is updated successfully</response>
        /// <response code="401">When the caller sends an invalid JWT</response>
        /// <response code="403">When the caller does not a super admin</response>
        /// <response code="404">When the role does not exist</response>
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("update-role-permissions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<string>>> UpdatePermissionsForRole(
            [FromBody] RolePermissionsDto payload)
        {
            var result = await _permissionService.UpdatePermissionsForRoleAsync(payload);
            return StatusCode(result.StatusCode, result);
        }
    }
}
