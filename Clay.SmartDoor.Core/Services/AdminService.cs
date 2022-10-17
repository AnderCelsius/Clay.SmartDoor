using Clay.SmartDoor.Core.DTOs.ActivityLogs;
using Clay.SmartDoor.Core.DTOs.Admin;
using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Constants;
using Clay.SmartDoor.Core.Models.Enums;
using Clay.SmartDoor.Core.Models.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Transactions;

namespace Clay.SmartDoor.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public AdminService(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            ILogger logger
            )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> AddUserAsync(NewUserRequest requestModel)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(requestModel.Email);
                if (userExist != null)
                {
                    return ApiResponse<string>.Fail(AuthenticationMessage.User_Already_Exist);
                }

                var user = new AppUser
                {
                    FirstName = requestModel.FirstName,
                    LastName = requestModel.LastName,
                    Email = requestModel.Email,
                    UserName = requestModel.Email,
                    IsActive = true,
                    AccessGroupId = requestModel.GroupId,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now,
                    CreatedBy = requestModel.CreatedBy,
                };

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.User_Created,
                    ActionBy = requestModel.CreatedBy
                };

                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    scope.Dispose();
                    return ApiResponse<string>.Fail(GetErrors(result));
                }
                await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                await _unitOfWork.ActivityLogs.AddAsync(activityLog);
                await _unitOfWork.SaveAsync();

                scope.Complete();

                return ApiResponse<string>.Success(
                    ApiResponseMesage.Created_Successfully, 
                    ApiResponseMesage.Created_Successfully);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message);
            }
        }

        public async Task<ApiResponse<string>> AddGroupAsync(string groupName, string userId)
        {
            try
            {
                var groupExist = await _unitOfWork.AccessGroups.GetAsync(g => g.Name == groupName);
                if(groupExist != null)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Fail_Already_Exist_Message);
                }

                var accessGroup = new AccessGroup
                {
                    Name = groupName,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now,
                    IsActive = true,
                    CreatedBy = userId
                };

                var activilog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Group_Created,
                    ActionBy = userId,
                };

                await _unitOfWork.AccessGroups.AddAsync(accessGroup);
                await _unitOfWork.ActivityLogs.AddAsync(activilog);

                var saveResult = await _unitOfWork.SaveAsync();


                _logger.Information(saveResult > 0 ? Constants.Generic_Save_Success_Message
                    : Constants.Generic_Failure_Message);

                return ApiResponse<string>.Success(ApiResponseMesage.Created_Successfully,ApiResponseMesage.Ok_Result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message);
            }
        }

        public async Task<ApiResponse<IEnumerable<AccessGroup>>> GetAllAccessGroupsAsync(GroupState groupState)
        {
            try
            {
                IQueryable<AccessGroup> accessGroups;

                if(groupState == GroupState.All)
                {
                    accessGroups = _unitOfWork.AccessGroups.GetAll();
                }
                else
                {
                    var isActive = groupState == GroupState.Active;
                    accessGroups = _unitOfWork.AccessGroups.GetAll(g => g.IsActive == isActive);
                }

                return new ApiResponse<IEnumerable<AccessGroup>>
                {
                    Data = await accessGroups.ToListAsync(),
                    Succeeded = true,
                    StatusCode = 200
                };

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<IEnumerable<AccessGroup>>.Fail(Constants.Generic_Operation_Failed_Message);
            }
        }

        public async Task<ApiResponse<string>> AddDoorToAcessGroupAsync(DoorAccessRequest requestModel, string userId)
        {
            try
            {
                var door = await _unitOfWork.Doors.GetAsync(d => d.Id == requestModel.DoorId);
                if(door == null)
                {
                    return ApiResponse<string>.Fail(DoorMessage.Not_Found);
                }

                var doorExistInGroup = await _unitOfWork.DoorAssignments.GetAsync(
                    d => d.DoorId == requestModel.DoorId && d.AccessGroupId == requestModel.GroupId && d.Assigned);

                if(doorExistInGroup != null)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Fail_Already_Exist_Message);
                }

                var doorAssignment = new DoorAssignment
                {
                    DoorId = requestModel.DoorId,
                    AccessGroupId = requestModel.GroupId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now,
                    Assigned = true
                };

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Door_Added_To_Group,
                    ActionBy = userId,
                    DoorId = door.Id,
                    DoorTag = door.NameTag,
                    Floor = door.Floor,
                    Building = door.Building,
                };

                await _unitOfWork.DoorAssignments.AddAsync(doorAssignment);
                await _unitOfWork.ActivityLogs.AddAsync(activityLog);

                var saveResult = await _unitOfWork.SaveAsync();

                _logger.Information(saveResult > 0 ? Constants.Generic_Save_Success_Message
                   : Constants.Generic_Failure_Message);

                return ApiResponse<string>.Success(ApiResponseMesage.Created_Successfully, ApiResponseMesage.Ok_Result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message);
            }
        }

        public async Task<ApiResponse<IEnumerable<ActivityLog>>> GetUserActivityLogAsync(ActivityLogsRequest requestModel)
        {
            try
            {
                var logsQuery = _unitOfWork.ActivityLogs
                    .GetUserActivityLogs(requestModel.UserId, requestModel.FromDate, requestModel.ToDate);

                var paginatedLogs = await PagedList<ActivityLog>.ToPagedListAsync(
                                                logsQuery, requestModel.PageNumber, requestModel.PageSize);

                return ApiResponse<IEnumerable<ActivityLog>>.Success(Constants.Generic_Success_Message, paginatedLogs);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<IEnumerable<ActivityLog>>.Fail(Constants.Generic_Operation_Failed_Message);
            }
        }

        /// <summary>
        /// Stringify and returns all the identity errors
        /// </summary>
        /// <param name="result"></param>
        /// <returns>Identity Errors</returns>
        private static string GetErrors(IdentityResult result)
        {
            return result.Errors.Aggregate(string.Empty, (current, err) => current + err.Description + "\n");
        }

        public async Task<ApiResponse<string>> RemoveDoorFromAccessGroupAsync(DoorAccessRequest requestModel, string userId)
        {
            try
            {
                var door = await _unitOfWork.Doors.GetAsync(d => d.Id == requestModel.DoorId);
                if (door == null)
                {
                    return ApiResponse<string>.Fail(DoorMessage.Not_Found);
                }

                var assignedDoor = await _unitOfWork.DoorAssignments.GetAsync(
                    d => d.DoorId == requestModel.DoorId && d.AccessGroupId == requestModel.GroupId && d.Assigned);

                if (assignedDoor == null)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Fail_Does_Not_Exist_Message);
                }

                assignedDoor.Assigned = false;

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Door_Removed_From_Group,
                    ActionBy = userId,
                    DoorId = assignedDoor.DoorId,
                    DoorTag = door.NameTag,
                    Floor = door.Floor,
                    Building = door.Building,
                };

                _unitOfWork.DoorAssignments.Update(assignedDoor);
                await _unitOfWork.ActivityLogs.AddAsync(activityLog);

                var saveResult = await _unitOfWork.SaveAsync();

                _logger.Information(saveResult > 0 ? Constants.Generic_Save_Success_Message
                   : Constants.Generic_Failure_Message);

                return ApiResponse<string>.Success(ApiResponseMesage.Created_Successfully, ApiResponseMesage.Ok_Result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message);
            }
        }
    }
}
