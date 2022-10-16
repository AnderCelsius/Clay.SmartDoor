using Clay.SmartDoor.Core.Dtos;
using Clay.SmartDoor.Core.Models;

namespace Clay.SmartDoor.Core.Interfaces.CoreServices
{
    public interface IDoorService
    {
        Task<ApiResponse<string>> CreateNewDoorAsync(CreateDoorRecord door);
    }
}
