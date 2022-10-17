using Clay.SmartDoor.Api.Bindings;
using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clay.SmartDoor.Api.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("api/v1/doors")]
    [ApiController]
    public class DoorsController : ControllerBase
    {
        private readonly IDoorService _doorService;

        public DoorsController(IDoorService doorService)
        {
            _doorService = doorService;
        }

        /// <summary>
        /// Adds a new door to the database.
        /// </summary>
        /// <param name="doorModel"></param>
        /// <param name="userId">The user calling the endpoint.</param>
        /// <returns></returns>
        /// <response code="201">If the door is created</response>
        /// <response code="401">when caller is not calling this enpoint with the right bearer token</response>
        /// <response code="403">When caller does not belong to the required group to create door</response>
        [Route("add-door")]
        [HttpPost]
        [Authorize(Policy = Permissions.Access.Create)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Add(
            [ModelBinder(BinderType = typeof(AuthenticatedUserIdBinder))] string userId,
            [FromBody] CreateDoorRecord doorModel)
        {
            var result = await _doorService.CreateNewDoorAsync(doorModel, userId);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Grants user access to exit a door if they pass all security requirements
        /// </summary>
        /// <param name="userId">The user calling the endpoint</param>
        /// <param name="doorId">The Id of the door to be exited</param>
        /// <returns></returns>
        /// <response code="200">If the user successfully gets access</response>
        /// <response code="401">when caller is not calling this enpoint with the right bearer token</response>
        /// <response code="403">When caller does not belong to the required group to access door</response>
        [Route("exit-door")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Exit(
            [ModelBinder(BinderType = typeof(AuthenticatedUserIdBinder))] string userId,
            string doorId)
        {
            var result = await _doorService.ExitDoorAsync(doorId, userId);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Retrieves the details of all doors.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">When the API call completes</response>
        /// <response code="401">when caller is not calling this enpoind with the right bearer token</response> 
        [Route("get-door")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<IEnumerable<DoorDetails>>>> Get()
        {
            var result = await _doorService.GetDoorsAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Grants user access to a given door if they pass all security requirements
        /// </summary>
        /// <param name="userId">The Id of the user calling the endpoint.</param>
        /// <param name="doorModel"></param>
        /// <returns></returns>
        /// <response code="202">If the user successfully gets access</response>
        /// <response code="401">when caller is not calling this enpoint with the right bearer token</response>
        /// <response code="403">When caller does not belong to the required group to access door</response>
        [Route("open-door")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Open(
            [ModelBinder(BinderType = typeof(AuthenticatedUserIdBinder))] string userId,
            [FromBody] DoorAccessRequest doorModel)
        {
            var result = await _doorService.OpenDoorAsync(doorModel, userId);
            return StatusCode(result.StatusCode, result);
        }


    }
}
