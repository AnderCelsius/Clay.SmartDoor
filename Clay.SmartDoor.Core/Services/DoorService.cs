using Clay.SmartDoor.Core.Dtos;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Constants;
using Serilog;

namespace Clay.SmartDoor.Core.Services
{
    public class DoorService : IDoorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public DoorService(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> CreateNewDoorAsync(CreateDoorRecord model)
        {
            try
            {
                _logger.Information("Beginning add operation...");
                var door = model.ToDoor(DateTime.Now, DateTime.Now);

                // Create door
                await _unitOfWork.Doors.AddAsync(door);
                _logger.Information("Door added");

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Door_Created,
                    ActionBy = model.CreatorId,
                    DoorId = door.Id,
                    Building = model.Building,
                    Floor = model.Floor,
                    DoorTag = door.NameTag
                };

                await _unitOfWork.ActivityLogs.AddAsync(activityLog);
                _logger.Information("Activity logged");

                var saveResult = await _unitOfWork.SaveAsync();
                _logger.Information(saveResult > 0 ? "Changes persisted to database" : "Failed to save");

                return ApiResponse<string>.Success(ApiResponseMesage.Created_Successfully, door.Id);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(ApiResponseMesage.Failed_To_Create);
            }
        }
    }
}
