using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Extensions;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;

namespace Clay.SmartDoor.Core.Services
{
    public class DoorService : IDoorService
    {
        private readonly UserManager<AppUser> _userManger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public DoorService(
            UserManager<AppUser> userManger,
            IUnitOfWork unitOfWork, ILogger logger)
        {
            _userManger = userManger;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> CreateNewDoorAsync(CreateDoorRecord model, string creatorId)
        {
            try
            {
                _logger.Information(Constants.Generic_Begin_Operation_Message);
                var door = model.ToDoor(DateTime.Now, DateTime.Now, creatorId);

                var doors = _unitOfWork.Doors.GetAll();

                bool exists = doors.Where(x => x.NameTag == model.NameTag).Any();

                if (exists)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Fail_Already_Exist_Message, 400);
                }

                // Create door
                await _unitOfWork.Doors.AddAsync(door);
                _logger.Information(ActivityDescriptions.Door_Created);

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Door_Created,
                    ActionBy = creatorId,
                    DoorId = door.Id,
                    Building = model.Building,
                    Floor = model.Floor,
                    DoorTag = door.NameTag,
                };

                await _unitOfWork.ActivityLogs.AddAsync(activityLog);
                _logger.Information(Constants.Generic_Activity_Logged_Message);

                var saveResult = await _unitOfWork.SaveAsync();

                _logger.Information(saveResult > 0 ? Constants.Generic_Save_Success_Message
                    : Constants.Generic_Failure_Message);

                return ApiResponse<string>.Success(ApiResponseMesage.Created_Successfully, door.Id, 201);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(ApiResponseMesage.Failed_To_Create, 400);
            }
        }
    
        public async Task<ApiResponse<string>> ExitDoorAsync(string doorId, string userId)
        {
            try
            {
                _logger.Information(Constants.Generic_Begin_Operation_Message);
                var door = await _unitOfWork.Doors.GetDoorAsync(doorId);

                if (door == null)
                {
                    _logger.Information(Constants.Generic_Operation_Failed_Message);
                    return ApiResponse<string>.Fail(DoorMessage.Not_Found, (int)HttpStatusCode.NotFound);
                }

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Exit_Door,
                    ActionBy = userId,
                    DoorId = doorId,
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

        public async Task<ApiResponse<DoorDetails>> GetDoorByIdAsync(string doorId)
        {
            try
            {
                _logger.Information(Constants.Generic_Begin_Operation_Message);
                var door = await _unitOfWork.Doors.GetDoorAsync(doorId);

                if(door == null)
                {
                    return ApiResponse<DoorDetails>.Fail(DoorMessage.Not_Found);
                }
                _logger.Information(Constants.Generic_Success_Message);
                return ApiResponse<DoorDetails>.Success(ApiResponseMesage.Ok_Result, door.ToDoorDetails());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<DoorDetails>.Fail(Constants.Generic_Operation_Failed_Message, 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<DoorDetails>>> GetDoorsAsync()
        {
            try
            {
                _logger.Information(Constants.Generic_Begin_Operation_Message);
                var doorsQuery = _unitOfWork.Doors.GetAllDoorsAsync();
                var doorDetailsList = await doorsQuery.Select(door => door.ToDoorDetails()).ToListAsyncSafe();

                _logger.Information(Constants.Generic_Success_Message);
                return ApiResponse<IEnumerable<DoorDetails>>.Success(ApiResponseMesage.Ok_Result, doorDetailsList);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<IEnumerable<DoorDetails>>.Fail(Constants.Generic_Operation_Failed_Message, 500);
            }
        }

        public async Task<ApiResponse<string>> OpenDoorAsync(string doorId, string userId)
        {
            try
            {
                _logger.Information(Constants.Generic_Begin_Operation_Message);
                var door = await _unitOfWork.Doors.GetDoorAsync(doorId);

                if(door == null)
                {
                    _logger.Information(Constants.Generic_Operation_Failed_Message);
                    return ApiResponse<string>.Fail(DoorMessage.Not_Found);
                }

                var user = await _userManger.FindByIdAsync(userId);

                var assignedDoor = await _unitOfWork.DoorAssignments.GetDoorAssignmentAsync(doorId, user.AccessGroupId);

                if (assignedDoor == null)
                {
                    var failedActivityLog = new ActivityLog
                    {
                        Time = DateTime.Now,
                        Description = ActivityDescriptions.Access_Denied,
                        ActionBy = userId,
                        DoorId = doorId,
                        Building = door.Building,
                        Floor = door.Floor,
                        DoorTag = door.NameTag
                    };

                    await _unitOfWork.ActivityLogs.AddAsync(failedActivityLog);
                    await _unitOfWork.SaveAsync();

                    _logger.Information(AuthenticationMessage.Forbidden);
                    return ApiResponse<string>.Fail(AuthenticationMessage.Forbidden, 403);
                }

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Door_Opened,
                    ActionBy = userId,
                    DoorId = doorId,
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
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message, 400);
            }
        }
    }
}
