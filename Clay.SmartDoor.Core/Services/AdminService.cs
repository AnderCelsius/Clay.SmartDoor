using Clay.SmartDoor.Core.DTOs.ActivityLogs;
using Clay.SmartDoor.Core.DTOs.Admin;
using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Extensions;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Core.Models;
using Clay.SmartDoor.Core.Models.Constants;
using Clay.SmartDoor.Core.Models.Enums;
using Clay.SmartDoor.Core.Models.Pagination;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Net;
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

        public async Task<ApiResponse<string>> AddAccessGroupAsync(NewAccessGroup requestModel, string actionBy)
        {
            try
            {
                _logger.Information(Constants.Generic_Begin_Operation_Message);
                var groupExist = await _unitOfWork.AccessGroups.GetAccessGroupByNameAsync(requestModel.GroupName);
                if (groupExist != null)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Fail_Already_Exist_Message, (int)HttpStatusCode.BadRequest);
                }

                var accessGroup = new AccessGroup
                {
                    Name = requestModel.GroupName,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now,
                    IsActive = true,
                    CreatedBy = actionBy
                };

                var activilog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Group_Created,
                    ActionBy = actionBy,
                };

                await _unitOfWork.AccessGroups.AddAsync(accessGroup);
                _logger.Information(ActivityDescriptions.AccessGoup_Added);

                await _unitOfWork.ActivityLogs.AddAsync(activilog);
                _logger.Information(ActivityDescriptions.Activity_Logged);


                var saveResult = await _unitOfWork.SaveAsync();

                _logger.Information(saveResult > 0 ? Constants.Generic_Save_Success_Message
                    : Constants.Generic_Failure_Message);

                return ApiResponse<string>.Success(ApiResponseMesage.Created_Successfully, ApiResponseMesage.Ok_Result, 201);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message, 400);
            }
        }
        public async Task<ApiResponse<string>> AddDoorToAcessGroupAsync(DoorAccessRequest requestModel, string actionBy)
        {
            try
            {
                var door = await _unitOfWork.Doors.GetDoorAsync(requestModel.DoorId);
                if (door == null)
                {
                    return ApiResponse<string>.Fail(DoorMessage.Not_Found);
                }

                var group = await _unitOfWork.AccessGroups.GetAccessGroupByIdAsync(requestModel.AccessGroupId);
                if (group == null)
                {
                    return ApiResponse<string>.Fail(AccessGroupMessage.Not_Found);
                }

                var doorExistInGroup = await _unitOfWork.DoorAssignments.GetDoorAssignmentAsync(requestModel.DoorId, requestModel.AccessGroupId);

                if (doorExistInGroup != null)
                {
                    return ApiResponse<string>.Fail(DoorMessage.Door_Create_Fail_Exist_Because_Found, 400);
                }

                var doorAssignment = new DoorAssignment
                {
                    DoorId = requestModel.DoorId,
                    AccessGroupId = requestModel.AccessGroupId,
                    CreatedBy = actionBy,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now,
                    Assigned = true
                };

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Door_Added_To_Group,
                    ActionBy = actionBy,
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

                return ApiResponse<string>.Success(DoorMessage.Add_to_Group_Success, DoorMessage.Add_to_Group_Success, 201);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message, 400);
            }
        }
        public async Task<ApiResponse<string>> AddUserAsync(NewUserRequest requestModel, string actionBy)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(requestModel.Email);
                if (userExist != null)
                {
                    return ApiResponse<string>.Fail(AuthenticationMessage.User_Already_Exist, 400);
                }

                // Access Group check
                var accessGroup = await _unitOfWork.AccessGroups.GetAccessGroupByIdAsync(requestModel.AccessGroupId);
                if (accessGroup == null)
                {
                    return ApiResponse<string>.Fail(AccessGroupMessage.Not_Found);
                }

                var user = new AppUser
                {
                    FirstName = requestModel.FirstName,
                    LastName = requestModel.LastName,
                    Email = requestModel.Email,
                    UserName = requestModel.Email,
                    IsActive = true,
                    AccessGroupId = requestModel.AccessGroupId,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now,
                    CreatedBy = actionBy,
                };

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.User_Created,
                    ActionBy = actionBy
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
                    ApiResponseMesage.Created_Successfully,
                    201);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message, 400);
            }
        }
        public async Task<ApiResponse<string>> UpdateUserAccessGroup(UpdateUserAccessGroup requestModel, string actionBy)
        {
            try
            {
                var oldAccessGroup = await _unitOfWork.AccessGroups.GetAccessGroupByIdAsync(requestModel.OldAccessGroupId);
                if (oldAccessGroup == null)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Not_Found_Message);
                }

                var newAccessGroup = await _unitOfWork.AccessGroups.GetAccessGroupByIdAsync(requestModel.NewAccessGroupId);
                if (newAccessGroup == null)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Not_Found_Message);
                }

                var foundUser = await _userManager.FindByIdAsync(requestModel.UserId);

                if (foundUser == null)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Not_Found_Message);
                }

                bool userExistInGroup = false;
                foreach(var user in oldAccessGroup.Users)
                {
                    if(user.Id == foundUser.Id)
                    {
                        userExistInGroup = true;
                        oldAccessGroup.Users.Remove(user);
                        break;
                    }
                }

                if (!userExistInGroup)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Fail_User_Does_Not_Belong_Message, 400);
                }

                

                newAccessGroup.Users.Add(foundUser);

                _unitOfWork.AccessGroups.Update(oldAccessGroup);
                _unitOfWork.AccessGroups.Update(newAccessGroup);
                var saveResult = await _unitOfWork.SaveAsync();

                _logger.Information(saveResult > 0 ? Constants.Generic_Save_Success_Message
                   : Constants.Generic_Failure_Message);

                return ApiResponse<string>.Success(ApiResponseMesage.User_Group_Update_Success, ApiResponseMesage.User_Group_Update_Success);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message, 400);
            }
        }
        public async Task<ApiResponse<IEnumerable<AccessGroupResponse>>> GetAllAccessGroupsAsync(GroupState groupState)
        {
            try
            {
                IQueryable<AccessGroup> accessGroupsQuery;

                if (groupState == GroupState.All)
                {
                    accessGroupsQuery = _unitOfWork.AccessGroups.GetAll();
                }
                else
                {
                    var isActive = groupState == GroupState.Active;
                    accessGroupsQuery = _unitOfWork.AccessGroups.GetAccessGroupsByActiveStatusAsync(isActive);
                }

                var accessGroup = await accessGroupsQuery.ToListAsyncSafe();
                return new ApiResponse<IEnumerable<AccessGroupResponse>>
                {
                    Data = accessGroup.Select(x => x.ToAccessGroupResponse()).ToList(),
                    Succeeded = true,
                    StatusCode = 200,
                    Message = ApiResponseMesage.Ok_Result
                };

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<IEnumerable<AccessGroupResponse>>.Fail(Constants.Generic_Operation_Failed_Message, 400);
            }
        }

        public async Task<ApiResponse<IEnumerable<ActivityLogDetails>>> GetUserActivityLogAsync(ActivityLogsRequest requestModel)
        {
            try
            {
                var userExist = await _userManager.FindByIdAsync(requestModel.UserId);
                if (userExist == null)
                {
                    return ApiResponse<IEnumerable<ActivityLogDetails>>.Fail(Constants.Generic_Fail_User_Not_Found_Message);
                }

                var logsQuery = _unitOfWork.ActivityLogs
                    .GetUserActivityLogs(requestModel.UserId, requestModel.FromDate, requestModel.ToDate);

                var paginatedLogs = await PagedList<ActivityLog>.ToPagedListAsync(
                                                logsQuery, requestModel.PageNumber, requestModel.PageSize);

                var activityLogDetails = paginatedLogs.Select(x => x.ToActivityLogDetails()).ToList();

                return ApiResponse<IEnumerable<ActivityLogDetails>>.Success(Constants.Generic_Success_Message, activityLogDetails);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<IEnumerable<ActivityLogDetails>>.Fail(Constants.Generic_Operation_Failed_Message, 400);
            }
        }

        public async Task<ApiResponse<string>> RemoveDoorFromAccessGroupAsync(DoorAccessRequest requestModel, string actionBy)
        {
            try
            {
                var door = await _unitOfWork.Doors.GetDoorAsync(requestModel.DoorId);
                if (door == null)
                {
                    return ApiResponse<string>.Fail(DoorMessage.Not_Found);
                }

                var assignedDoor = await _unitOfWork.DoorAssignments.GetDoorAssignmentAsync(requestModel.DoorId, requestModel.AccessGroupId);

                if (assignedDoor == null)
                {
                    return ApiResponse<string>.Fail(Constants.Generic_Fail_Does_Not_Exist_Message, 400);
                }

                assignedDoor.Assigned = false;

                var activityLog = new ActivityLog
                {
                    Time = DateTime.Now,
                    Description = ActivityDescriptions.Door_Removed_From_Group,
                    ActionBy = actionBy,
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

                return ApiResponse<string>.Success(ApiResponseMesage.Ok_Result, ApiResponseMesage.Ok_Result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return ApiResponse<string>.Fail(Constants.Generic_Operation_Failed_Message, 400);
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

       
    }
}
