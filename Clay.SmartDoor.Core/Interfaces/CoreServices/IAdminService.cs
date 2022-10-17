using Clay.SmartDoor.Core.DTOs.ActivityLogs;
using Clay.SmartDoor.Core.DTOs.Admin;
using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Enums;

namespace Clay.SmartDoor.Core.Interfaces.CoreServices
{
    public interface IAdminService
    {
        Task<ApiResponse<string>> AddAccessGroupAsync(string groupName, string userId);
        Task<ApiResponse<string>> AddDoorToAcessGroupAsync(DoorAccessRequest requestModel, string userId);
        Task<ApiResponse<string>> AddUserAsync(NewUserRequest model, string createdBy);
        Task<ApiResponse<IEnumerable<AccessGroup>>> GetAllAccessGroupsAsync(GroupState groupState);
        Task<ApiResponse<IEnumerable<ActivityLogDetails>>> GetUserActivityLogAsync(ActivityLogsRequest requestModel);
        Task<ApiResponse<string>> RemoveDoorFromAccessGroupAsync(DoorAccessRequest requestModel, string userId);
    }
}
