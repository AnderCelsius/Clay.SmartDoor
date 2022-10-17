using Clay.SmartDoor.Api.Bindings;
using Clay.SmartDoor.Core.DTOs.Admin;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Microsoft.AspNetCore.Mvc;

namespace Clay.SmartDoor.Api.Controllers
{
    [Route("api/v1/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">The Id of the user calling the endpoint</param>
        /// <param name="groupName">The name of the Access Group to be created</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add-access-group")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AddAccessGroup(
            [ModelBinder(BinderType = typeof(AppUserIdBinder))] string userId,
            string groupName)
        {
            var result = await _adminService.AddAccessGroupAsync(groupName, userId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add-user")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AddUser(
            [ModelBinder(BinderType = typeof(AppUserIdBinder))] string userId,
            NewUserRequest requestModel)
        {
            var result = await _adminService.AddUserAsync(requestModel, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
