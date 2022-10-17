using Clay.SmartDoor.Core.Dtos.Doors;
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
        /// Adds a new door 
        /// </summary>
        /// <param name="doorModel"></param>
        /// <returns></returns>
        [Route("AddDoor")]
        [HttpPost]
        [Authorize(Policy = Permissions.Door.Create)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddDoor([FromBody] CreateDoorRecord doorModel)
        {
            var result = await _doorService.CreateNewDoorAsync(doorModel);
            return StatusCode(result.StatusCode, result);
        }
    }
}
