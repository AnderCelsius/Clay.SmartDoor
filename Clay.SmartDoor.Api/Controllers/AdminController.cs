using Clay.SmartDoor.Api.Bindings;
using Clay.SmartDoor.Core.DTOs.ActivityLogs;
using Clay.SmartDoor.Core.DTOs.Admin;
using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clay.SmartDoor.Api.Controllers
{
    [Route("api/v1/admin")]
    [Authorize]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }


        /// <summary>
        /// Creates a new Access Group. 
        /// 
        /// This will additionally log the activity.
        /// </summary>
        /// <param name="userId">The Id of the user calling the endpoint</param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// <response code="201">When the group is successfully created</response>
        /// <response code="401">If jwt token provided is invalid.</response>
        /// <response code="403">If caller does not have the permission to create.</response>
        [HttpPost]
        [Route("add-access-group")]
        [Authorize(Policy = "Access.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AddAccessGroup(
            [ModelBinder(BinderType = typeof(AuthenticatedUserIdBinder))] string userId,
            [FromBody] NewAccessGroup requestModel)
        {
            var result = await _adminService.AddAccessGroupAsync(requestModel, userId);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Adds the door that corresponds to the given to Id to the Access Group belonging to the groupId
        /// provided.
        /// 
        /// For this operation to work, the door must not already belong to the group and both the door
        /// and the group must exist.
        /// 
        /// This will additionally log the activity.
        /// </summary>
        /// <param name="userId">The Id of the user calling the api.</param>
        /// <param name="requestModel">Contains the DoorId and AccessGroupId</param>
        /// <returns></returns>
        /// <response code="200">When the door is successfully added to the access group.</response>
        /// <response code="401">If jwt token provided is invalid.</response>
        /// <response code="403">If caller does not have the permission to create.</response>
        [HttpPost]
        [Route("add-door-to-access-group")]
        [Authorize(Policy = "Access.Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AddDoorToAccessGroup(
            [ModelBinder(BinderType = typeof(AuthenticatedUserIdBinder))] string userId,
            DoorAccessRequest requestModel)
        {
            var result = await _adminService.AddDoorToAcessGroupAsync(requestModel, userId);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Creates a new user
        /// 
        /// This will additionally log the activity.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// <response code="201">When the user is successfully created</response>
        /// <response code="401">If jwt token provided is invalid.</response>
        /// <response code="403">If caller does not have the permission to create user.</response>
        [HttpPost]
        [Route("add-user")]
        [Authorize(Policy = "User.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AddUser(
            [ModelBinder(BinderType = typeof(AuthenticatedUserIdBinder))] string userId,
            NewUserRequest requestModel)
        {
            var result = await _adminService.AddUserAsync(requestModel, userId);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Adds a user to access group
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// <response code="201">When the user is successfully added to the group</response>
        /// <response code="401">If jwt token provided is invalid.</response>
        /// <response code="403">If caller does not have the permission to create user.</response>
        [HttpPost]
        [Route("update-user-access-group")]
        [Authorize(Policy = "Access.Grant")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateUserAccessGroup(
            [ModelBinder(BinderType = typeof(AuthenticatedUserIdBinder))] string userId,
            UpdateUserAccessGroup requestModel)
        {
            var result = await _adminService.UpdateUserAccessGroup(requestModel, userId);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Retrievs all AccessGroups
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>GroupStates - <c>Active (default)</c>, <c>InActive</c>, <c>All</c></item>
        /// </list>
        /// </remarks>
        /// <param name="groupState">The applied filter for the state of the group.</param>
        /// <returns></returns>
        /// <response code="200">If data is successfully retrieved.</response>
        /// <response code="401">If jwt token provided is invalid.</response>
        [HttpGet]
        [Route("access-groups")]
        [Authorize(Policy = "Access.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAccessGroups(GroupState groupState)
        {
            var result = await _adminService.GetAllAccessGroupsAsync(groupState);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Fetches activity logs associated with the userId within a specified date range
        /// </summary>    
        /// <param name="requestModel"></param>
        /// <returns>A paginated result of the users activities.</returns>
        /// <response code="200">If data is successfully retrieved.</response>
        /// <response code="401">If jwt token provided is invalid.</response>
        [HttpGet]
        [Route("user-activity-logs")]
        [Authorize(Policy = "Access.Read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ActivityLogDetails>>> GetUserActivityLogs(
            [FromQuery]ActivityLogsRequest requestModel)
        {
            var result = await _adminService.GetUserActivityLogAsync(requestModel);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Removes door from group.
        /// 
        /// Users belonging to the group will no longer have access to the door.
        /// </summary>
        /// <param name="userId">The user performing the operation</param>    
        /// <param name="requestModel"></param>
        /// <returns>A paginated result of the users activities.</returns>
        /// <response code="200">If the operation is successful</response>
        /// <response code="401">If jwt token provided is invalid.</response>
        /// <response code="403">If caller does not have the permission to perform the operation.</response>
        [HttpPost]
        [Route("remove-door-from-access-group")]
        [Authorize(Policy = "Access.Revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> RemoveDoorFromGroup(
            [ModelBinder(BinderType = typeof(AuthenticatedUserIdBinder))] string userId,
            [FromBody] DoorAccessRequest requestModel)
        {
            var result = await _adminService.RemoveDoorFromAccessGroupAsync(requestModel, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
