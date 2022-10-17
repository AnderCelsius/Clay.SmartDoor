using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Extensions;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Constants;
using Microsoft.EntityFrameworkCore;
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

                _logger.Information(saveResult > 0 ? Constants.Generic_Save_Success_Message
                    : Constants.Generic_Failure_Message);

                return ApiResponse<string>.Success(ApiResponseMesage.Created_Successfully, door.Id);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(ApiResponseMesage.Failed_To_Create);
            }
        }
    
        public async Task<ApiResponse<string>> ExitDoorAsync(ExitDoor model)
        {
            try
            {
                _logger.Information(Constants.Generic_Begin_Operation_Message);
                var door = await _unitOfWork.Doors.GetAsync(d => d.Id == model.DoorId);

                if (door == null)
                {
                    _logger.Information(Constants.Generic_Operation_Failed_Message);
                    return ApiResponse<string>.Fail(DoorMessage.Not_Found);
                }

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Door_Created,
                    ActionBy = model.UserId,
                    DoorId = model.DoorId,
                    Building = door.Building,
                    Floor = door.Floor,
                    DoorTag = door.NameTag
                };

                await _unitOfWork.ActivityLogs.AddAsync(activityLog);

                _logger.Information(ActivityDescriptions.Activity_Logged);

                var saveResult = await _unitOfWork.SaveAsync();

                _logger.Information(saveResult > 0 ? Constants.Generic_Save_Success_Message
                    : Constants.Generic_Failure_Message);

                return ApiResponse<string>.Success(ApiResponseMesage.Ok_Result, ApiResponseMesage.Ok_Result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(ApiResponseMesage.Failed_To_Create);
            }
        }

        public async Task<ApiResponse<IEnumerable<DoorDetails>>> GetDoorsAsync()
        {
            try
            {
                _logger.Information(Constants.Generic_Begin_Operation_Message);
                var doorsQuery = _unitOfWork.Doors.GetAll(orderBy: d => d.OrderByDescending(x => x.CreatedAt));
                var doorDetailsList = await doorsQuery.Select(door => door.ToDoorDetails()).ToListAsync();

                _logger.Information(Constants.Generic_Success_Message);
                return ApiResponse<IEnumerable<DoorDetails>>.Success(ApiResponseMesage.Ok_Result, doorDetailsList);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<IEnumerable<DoorDetails>>.Fail(Constants.Generic_Operation_Failed_Message);
            }
        }

        public async Task<ApiResponse<string>> OpenDoorAsync(DoorAccessRequest model, string userId)
        {
            try
            {
                _logger.Information(Constants.Generic_Begin_Operation_Message);
                var door = await _unitOfWork.Doors.GetAsync(d => d.Id == model.DoorId);

                if(door == null)
                {
                    _logger.Information(Constants.Generic_Operation_Failed_Message);
                    return ApiResponse<string>.Fail(DoorMessage.Not_Found);
                }
 
                var assignedDoor = await _unitOfWork.DoorAssignments
                    .GetAsync(da => da.DoorId == model.DoorId && da.AccessGroupId == model.GroupId && da.Assigned == true);

                if (assignedDoor == null)
                {
                    _logger.Information(AuthenticationMessage.Forbidden);
                    return ApiResponse<string>.Fail(AuthenticationMessage.Forbidden, 403);
                }


                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Door_Opened,
                    ActionBy = userId,
                    DoorId = model.DoorId,
                    Building = door.Building,
                    Floor = door.Floor,
                    DoorTag = door.NameTag
                };

                await _unitOfWork.ActivityLogs.AddAsync(activityLog);

                _logger.Information(ActivityDescriptions.Activity_Logged);

                var saveResult = await _unitOfWork.SaveAsync();

                _logger.Information(saveResult > 0 ? Constants.Generic_Save_Success_Message 
                    : Constants.Generic_Failure_Message);

                return ApiResponse<string>.Success(ApiResponseMesage.Ok_Result, ApiResponseMesage.Ok_Result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message);
            }
        }
    }
}
