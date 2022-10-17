using Clay.SmartDoor.Core.Dtos;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clay.SmartDoor.Api.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DoorsController : ControllerBase
    {
        private readonly IDoorService _doorService;

        public DoorsController(IDoorService doorService)
        {
            _doorService = doorService;
        }

        /// <summary>
        /// Add a new door.
        /// CreatorUserId is the ID of the user calling this endpoint.
        /// </summary>
        ///<response code="200">When the API call completes</response>
        /// <response code="401">when caller is not calling this enpoind with the right bearer token</response>
        ///
        [Route("AddDoor")]
        [HttpPost]
        [Authorize(Policy = Permissions.Door.Create)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddDoor([FromBody] CreateDoorRecord doorModel)
        {
            return Ok();
        }
    }
}
