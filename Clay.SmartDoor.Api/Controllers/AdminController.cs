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
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AddUser(NewUserRequest requestModel)
        {
            var result = await _adminService.AddUserAsync(requestModel);
            return StatusCode(result.StatusCode, result);
        }
    }
}
