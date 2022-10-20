using Clay.SmartDoor.Core.DTOs.ActivityLogs;
using Clay.SmartDoor.Core.DTOs.Admin;
using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Core.Models.Constants;
using Clay.SmartDoor.Core.Models.Enums;
using Clay.SmartDoor.Core.Services;
using Clay.SmartDoor.Test.Helper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Serilog;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Clay.SmartDoor.Test.Integration.Services
{
    public class AdminServiceTests
    {
        private readonly IAdminService _sut;
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IActivityLogRepository> _mockActivityLogRepo = new();
        private readonly Mock<IDoorAssignmentRepository> _mockDoorAssignmentRepo = new();
        private readonly Mock<ILogger> _mockLogger = new();
        public AdminServiceTests()
        {
            _mockUserManager = MockHelpers.MockUserManager<AppUser>(TestDataGenerator.DummyUsers);
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(x => x.ActivityLogs).Returns(_mockActivityLogRepo.Object);
            _mockUnitOfWork.Setup(x => x.DoorAssignments).Returns(_mockDoorAssignmentRepo.Object);
            _sut = new AdminService(_mockUserManager.Object, _mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddAccessGroupAsync_ShouldReturnFailResponse_WhenGroupNameExist()
        {
            // Arrange
            var groupName = "Test";
            var requestModel = new NewAccessGroup
            {
                GroupName = groupName,
            };
            var accessGroup = new AccessGroup
            {
                Name = groupName,
                IsActive = true,
                Users = TestDataGenerator.DummyUsers
            };
            _mockUnitOfWork.Setup(uow => uow.AccessGroups.GetAccessGroupByNameAsync(groupName)).ReturnsAsync(accessGroup);

            // Act
            var response = await _sut.AddAccessGroupAsync(requestModel, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.Message.ShouldBe(Constants.Generic_Fail_Already_Exist_Message);
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task AddAccessGroupAsync_ShouldReturnCreatedResponse_WhenGroupIsAddedSuccessfully()
        {
            // Arrange
            var groupName = "Test";
            var requestModel = new NewAccessGroup
            {
                GroupName = groupName,
            };
            var accessGroup = new AccessGroup
            {
                Name = groupName,
                IsActive = true,
                Users = TestDataGenerator.DummyUsers
            };

            AccessGroup noAccessGroup = null!;
            _mockUnitOfWork.Setup(uow => uow.AccessGroups.GetAccessGroupByNameAsync(groupName)).ReturnsAsync(noAccessGroup);

            // Act
            var response = await _sut.AddAccessGroupAsync(requestModel, TestDataGenerator.ActionBy);

            // Assert
            response.Message.ShouldBe(ApiResponseMesage.Created_Successfully);
            response.StatusCode.ShouldBe((int)HttpStatusCode.Created);
            response.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task AddAccessGroupAsync_ShouldHandleExceptionAndReturnFailedResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var groupName = "Test";
            var requestModel = new NewAccessGroup
            {
                GroupName = groupName,
            };
            var accessGroup = new AccessGroup
            {
                Name = groupName,
                IsActive = true,
                Users = TestDataGenerator.DummyUsers
            };

            AccessGroup noAccessGroup = null!;
            _mockUnitOfWork.Setup(uow => uow.AccessGroups.GetAccessGroupByNameAsync(groupName)).ReturnsAsync(noAccessGroup);
            _mockUnitOfWork.Setup(uow => uow.ActivityLogs.AddAsync(It.IsAny<ActivityLog>())).Throws(new Exception());

            // Act
            var response = await _sut.AddAccessGroupAsync(requestModel, TestDataGenerator.ActionBy);

            // Assert
            response.Message.ShouldBe(Constants.Generic_Operation_Failed_Message);
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task AddDoorToAcessGroupAsync_ShouldReturnFailResponse_WhenDoorDoesNotExist()
        {
            // Arrange
            var model = new DoorAccessRequest
            {
                DoorId = TestDataGenerator.Default_Id,
                AccessGroupId = TestDataGenerator.Default_AccessGroup
            };

            Door foundDoor = null!;
            _mockUnitOfWork.Setup(uow => uow.Doors.GetDoorAsync(model.DoorId)).ReturnsAsync(foundDoor);

            // Act
            var response = await _sut.AddDoorToAcessGroupAsync(model, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.Message.ShouldBe(DoorMessage.Not_Found);
            response.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task AddDoorToAcessGroupAsync_ShouldReturnFailResponse_WhenDoorAlreadyBelongsToGroup()
        {
            // Arrange
            var model = new DoorAccessRequest
            {
                DoorId = TestDataGenerator.Default_Id,
                AccessGroupId = TestDataGenerator.Default_AccessGroup
            };

            var doorAssignment = new DoorAssignment();

            _mockUnitOfWork.Setup(uow => uow.Doors.GetDoorAsync(model.DoorId)).ReturnsAsync(TestDataGenerator.DefaultDoor);
            _mockUnitOfWork.Setup(uow => uow.DoorAssignments.GetDoorAssignmentAsync(model.DoorId, model.AccessGroupId)).ReturnsAsync(doorAssignment);

            // Act
            var response = await _sut.AddDoorToAcessGroupAsync(model, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.Message.ShouldBe(DoorMessage.Door_Create_Fail_Exist_Because_Found);
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task AddDoorToAcessGroupAsync_ShouldReturnCreatedResponse_WhenDoorIsAddedToGroupSuccessfully()
        {
            // Arrange
            var model = new DoorAccessRequest
            {
                DoorId = TestDataGenerator.Default_Id,
                AccessGroupId = TestDataGenerator.Default_AccessGroup
            };

            DoorAssignment doorAssignment = null!;

            _mockUnitOfWork.Setup(uow => uow.Doors.GetDoorAsync(model.DoorId)).ReturnsAsync(TestDataGenerator.DefaultDoor);
            _mockUnitOfWork.Setup(uow => uow.DoorAssignments.GetDoorAssignmentAsync(model.DoorId, model.AccessGroupId)).ReturnsAsync(doorAssignment);

            // Act
            var response = await _sut.AddDoorToAcessGroupAsync(model, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.Message.ShouldBe(DoorMessage.Add_to_Group_Success);
            response.StatusCode.ShouldBe((int)HttpStatusCode.Created);
            response.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task AddDoorToAcessGroupAsync_ShouldHandleExceptionAndReturnFailedResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var model = new DoorAccessRequest
            {
                DoorId = TestDataGenerator.Default_Id,
                AccessGroupId = TestDataGenerator.Default_AccessGroup
            };

            _mockUnitOfWork.Setup(uow => uow.Doors.GetDoorAsync(model.DoorId)).ReturnsAsync(TestDataGenerator.DefaultDoor);
            _mockUnitOfWork.Setup(uow => uow.DoorAssignments.GetDoorAssignmentAsync(model.DoorId, model.AccessGroupId)).Throws(new Exception());

            // Act
            var response = await _sut.AddDoorToAcessGroupAsync(model, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.Message.ShouldBe(Constants.Generic_Operation_Failed_Message);
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
        }
       
        [Fact]
        public async Task AddUserAsync_ShouldReturnFailResponse_WhenEmailExist()
        {
            // Arrange
            var data = new NewUserRequest
            {
                FirstName = TestDataGenerator.BasicUser.FirstName,
                LastName = TestDataGenerator.BasicUser.LastName,
                Email = TestDataGenerator.BasicUser.Email,
                Password = "Password@123",
                AccessGroupId = TestDataGenerator.Default_AccessGroup

            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(TestDataGenerator.BasicUser);

            // Act
            var response = await _sut.AddUserAsync(data, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.Message.ShouldBe(AuthenticationMessage.User_Already_Exist);
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task AddUserAsync_ShouldReturnCreatedResponse_WhenUserIsAddedSuccessfully()
        {
            // Arrange
            var data = new NewUserRequest
            {
                FirstName = TestDataGenerator.BasicUser.FirstName,
                LastName = TestDataGenerator.BasicUser.LastName,
                Email = TestDataGenerator.BasicUser.Email,
                Password = "Password@123",
                AccessGroupId = TestDataGenerator.Default_AccessGroup

            };

            AppUser notExistUser = null!;
            
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(notExistUser);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), Roles.Basic.ToString()));


            // Act
            var response = await _sut.AddUserAsync(data, TestDataGenerator.ActionBy);

            // Assert
            response.Message.ShouldBe(ApiResponseMesage.Created_Successfully);
            response.StatusCode.ShouldBe((int)HttpStatusCode.Created);
            response.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task AddUserAsync_ShouldHandleExceptionAndReturnFailedResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var data = new NewUserRequest
            {
                FirstName = TestDataGenerator.BasicUser.FirstName,
                LastName = TestDataGenerator.BasicUser.LastName,
                Email = TestDataGenerator.BasicUser.Email,
                Password = "Password@123",
                AccessGroupId = TestDataGenerator.Default_AccessGroup

            };

            AppUser notExistUser = null!;

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(notExistUser);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), Roles.Basic.ToString())).ThrowsAsync(new Exception());


            // Act
            var response = await _sut.AddUserAsync(data, TestDataGenerator.ActionBy);

            // Assert
            response.Message.ShouldBe(Constants.Generic_Operation_Failed_Message);
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task GetAllAccessGroupsAsync_ShouldReturnEmptyListAndOkResponse_WhenNoGroupExists()
        {
            // Arrange
            List<AccessGroup> accessGroups = new();
            _mockUnitOfWork.Setup(uow => uow.AccessGroups.GetAccessGroupsByActiveStatusAsync(true)).Returns(accessGroups.AsQueryable());

            // Act
            var result = await _sut.GetAllAccessGroupsAsync(GroupState.Active);

            // Assert
            result.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            result.Message.ShouldBe(ApiResponseMesage.Ok_Result);
            result.Data.Count().ShouldBe(0);
            result.Succeeded.ShouldBe(true);
            
        }

        [Fact]
        public async Task GetAllAccessGroupsAsync_ShouldReturnListOfDoorDetailsAndOkResponse_WhenDoorsExist()
        {
            // Arrange
            List<AccessGroup> accessGroups = new()
            {
                new AccessGroup { Id = TestDataGenerator.Default_AccessGroup, Name = "Default Group", IsActive = true},
                new AccessGroup { Id = TestDataGenerator.Default_SecureGroup_One, Name = "Secure Group", IsActive = true},
                new AccessGroup { Id = TestDataGenerator.Default_Id, Name = "Inactive Group", IsActive = false},
            };
            _mockUnitOfWork.Setup(uow => uow.AccessGroups.GetAccessGroupsByActiveStatusAsync(true)).Returns(accessGroups.AsQueryable().Where(x => x.IsActive == true));

            // Act
            var result = await _sut.GetAllAccessGroupsAsync(GroupState.Active);

            // Assert
            result.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            result.Message.ShouldBe(ApiResponseMesage.Ok_Result);
            result.Data.Count().ShouldBe(2);
            result.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task GetAllAccessGroupsAsync_ShouldHandleExceptionAndReturnFailedResponse_WhenExceptionIsThrown()
        {
            // Arrange
            List<AccessGroup> accessGroups = new()
            {
                new AccessGroup { Id = TestDataGenerator.Default_AccessGroup, Name = "Default Group", IsActive = true },
                new AccessGroup { Id = TestDataGenerator.Default_SecureGroup_One, Name = "Secure Group", IsActive = true },
                new AccessGroup { Id = TestDataGenerator.Default_Id, Name = "Inactive Group", IsActive = false },
            };
            _mockUnitOfWork.Setup(uow => uow.AccessGroups.GetAccessGroupsByActiveStatusAsync(true)).Throws(new Exception());

            // Act
            var response = await _sut.GetAllAccessGroupsAsync(GroupState.Active);

            // Assert
            response.Message.ShouldBe(Constants.Generic_Operation_Failed_Message);
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task GetUserActivityLogAsync_ShouldReturnEmptyListAndOkResponse_WhenActivityListIsEmpty()
        {
            // Arrange
            var request = new ActivityLogsRequest
            {
                UserId = TestDataGenerator.Default_Id,
                FromDate = DateTime.Now.AddDays(-1),
                ToDate = DateTime.Now.AddDays(1),
            };

            List<ActivityLog> logs = new();
            _mockUnitOfWork.Setup(uow => uow.ActivityLogs
                    .GetUserActivityLogs(TestDataGenerator.Default_Id, DateTime.Now, DateTime.Now))
                    .Returns(logs.AsQueryable());



            // Act
            var result = await _sut.GetUserActivityLogAsync(request);

            // Assert
            result.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            result.Message.ShouldBe(Constants.Generic_Success_Message);
            result.Data.Count().ShouldBe(0);
            result.Succeeded.ShouldBe(true);

        }

        [Fact]
        public async Task GetUserActivityLogAsync_ShouldReturnListOfActivitiesAndOkResponse_WhenRecordsExist()
        {
            // Arrange
            List<ActivityLog> logs = new()
            {
                TestDataGenerator.activityLog,
            };
            var request = new ActivityLogsRequest
            {
                UserId = TestDataGenerator.Default_Id,
                FromDate = DateTime.Now.AddDays(-1),
                ToDate = DateTime.Now.AddDays(1),
            };

            _mockUnitOfWork.Setup(uow => uow.ActivityLogs
                    .GetUserActivityLogs(request.UserId, request.FromDate, request.ToDate))
                    .Returns(logs.AsQueryable());
            
            // Act
            var result = await _sut.GetUserActivityLogAsync(request);

            // Assert
            result.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            result.Message.ShouldBe(Constants.Generic_Success_Message);
            result.Data.Count().ShouldBe(1);
            result.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task GetUserActivityLogAsync_ShouldHandleExceptionAndReturnFailedResponse_WhenExceptionIsThrown()
        {
            // Arrange
            List<ActivityLog> logs = new()
            {
                TestDataGenerator.activityLog,
            };
            var request = new ActivityLogsRequest
            {
                UserId = TestDataGenerator.Default_Id,
                FromDate = DateTime.Now.AddDays(-1),
                ToDate = DateTime.Now.AddDays(1),
            };

            _mockUnitOfWork.Setup(uow => uow.ActivityLogs
                    .GetUserActivityLogs(request.UserId, request.FromDate, request.ToDate))
                    .Throws(new Exception());

            // Act
            var response = await _sut.GetUserActivityLogAsync(request);

            // Assert
            response.Message.ShouldBe(Constants.Generic_Operation_Failed_Message);
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task RemoveDoorFromAccessGroupAsync_ShouldReturnFailResponse_WhenDoorDoesNotExist()
        {
            // Arrange
            var requestModel = new DoorAccessRequest
            {
                DoorId = TestDataGenerator.Default_Door_Id,
                AccessGroupId = TestDataGenerator.Default_AccessGroup
            };

            Door door = null!;
            _mockUnitOfWork.Setup(uow => uow.Doors.GetDoorAsync(It.IsAny<string>())).ReturnsAsync(door);

            // Act
            var response = await _sut.RemoveDoorFromAccessGroupAsync(requestModel, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.Message.ShouldBe(DoorMessage.Not_Found);
            response.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task RemoveDoorFromAccessGroupAsync_ShouldReturnFailedResponse_WhenDoorIsNotAssignedToGroup()
        {
            // Arrange
            var requestModel = new DoorAccessRequest
            {
                DoorId = TestDataGenerator.Default_Door_Id,
                AccessGroupId = TestDataGenerator.Default_AccessGroup
            };

            Door door = new()!;
            DoorAssignment doorAssignment = null!;
            _mockUnitOfWork.Setup(uow => uow.Doors.GetDoorAsync(It.IsAny<string>())).ReturnsAsync(door);
            _mockUnitOfWork.Setup(uow => uow.DoorAssignments
                        .GetDoorAssignmentAsync(requestModel.DoorId, requestModel.AccessGroupId))
                        .ReturnsAsync(doorAssignment);

            // Act
            var response = await _sut.RemoveDoorFromAccessGroupAsync(requestModel, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.Message.ShouldBe(Constants.Generic_Fail_Does_Not_Exist_Message);
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task RemoveDoorFromAccessGroupAsync_ShouldReturnCreatedResponse_WhenGroupIsAddedSuccessfully()
        {
            // Arrange
            var requestModel = new DoorAccessRequest
            {
                DoorId = TestDataGenerator.Default_Door_Id,
                AccessGroupId = TestDataGenerator.Default_AccessGroup
            };

            Door door = new()!;
            DoorAssignment doorAssignment = new()
            {
                DoorId = TestDataGenerator.Default_Door_Id,
                AccessGroupId = TestDataGenerator.Default_AccessGroup,
                Assigned = true
            };
            _mockUnitOfWork.Setup(uow => uow.Doors.GetDoorAsync(It.IsAny<string>())).ReturnsAsync(door);
            _mockUnitOfWork.Setup(uow => uow.DoorAssignments
                        .GetDoorAssignmentAsync(requestModel.DoorId, requestModel.AccessGroupId))
                        .ReturnsAsync(doorAssignment);

            // Act
            var response = await _sut.RemoveDoorFromAccessGroupAsync(requestModel, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.Message.ShouldBe(ApiResponseMesage.Ok_Result);
            response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            response.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task RemoveDoorFromAccessGroupAsync_ShouldHandleExceptionAndReturnFailedResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var requestModel = new DoorAccessRequest
            {
                DoorId = TestDataGenerator.Default_Door_Id,
                AccessGroupId = TestDataGenerator.Default_AccessGroup
            };

            Door door = new()!;
            DoorAssignment doorAssignment = new()
            {
                DoorId = TestDataGenerator.Default_Door_Id,
                AccessGroupId = TestDataGenerator.Default_AccessGroup,
                Assigned = true
            };
            _mockUnitOfWork.Setup(uow => uow.Doors.GetDoorAsync(It.IsAny<string>())).Throws(new Exception());
            // Act
            var response = await _sut.RemoveDoorFromAccessGroupAsync(requestModel, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.Message.ShouldBe(Constants.Generic_Operation_Failed_Message);
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task UpdateUserAccessGroup_ShouldReturnFailedResponse_IfAccessGroupIsNotFound()
        {
            // Arrange
            UpdateUserAccessGroup requestModel = new()
            {
                NewAccessGroupId = Guid.NewGuid().ToString(),
                OldAccessGroupId = Guid.NewGuid().ToString(),
                UserId = TestDataGenerator.Default_Id
            };

            AccessGroup accessGroup = null!;

            _mockUnitOfWork.Setup(x => x.AccessGroups.GetAccessGroupByIdAsync(It.IsAny<string>())).ReturnsAsync(accessGroup);

            // Act
            var response = await _sut.UpdateUserAccessGroup(requestModel, Guid.NewGuid().ToString());

            // Assert
            response.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
            response.Succeeded.ShouldBe(false);
            response.Data.ShouldBe(null);
            response.Message.ShouldBe(Constants.Generic_Not_Found_Message);
        }

        [Fact]
        public async Task UpdateUserAccessGroup_ShouldReturnFailedResponse_IfUserIsNotFound()
        {
            // Arrange
            UpdateUserAccessGroup requestModel = new()
            {
                NewAccessGroupId = Guid.NewGuid().ToString(),
                OldAccessGroupId = Guid.NewGuid().ToString(),
                UserId = TestDataGenerator.Default_Id
            };

            AccessGroup newAccessGroup = TestDataGenerator.SecureAccessGroup;
            AccessGroup oldAccessGroup = TestDataGenerator.OpenAccessGroup;
            AppUser user = null!;

            _mockUnitOfWork.Setup(x => x.AccessGroups.GetAccessGroupByIdAsync(requestModel.OldAccessGroupId)).ReturnsAsync(oldAccessGroup);
            _mockUnitOfWork.Setup(x => x.AccessGroups.GetAccessGroupByIdAsync(requestModel.NewAccessGroupId)).ReturnsAsync(newAccessGroup);
            _mockUserManager.Setup(x => x.FindByIdAsync(requestModel.UserId)).ReturnsAsync(user);
            // Act
            var response = await _sut.UpdateUserAccessGroup(requestModel, Guid.NewGuid().ToString());

            // Assert
            response.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
            response.Succeeded.ShouldBe(false);
            response.Data.ShouldBe(null);
            response.Message.ShouldBe(Constants.Generic_Not_Found_Message);
        }

        [Fact]
        public async Task UpdateUserAccessGroup_ShouldReturnFailedResponse_IfUserDoesNotBelongToOldAccessGroup()
        {
            // Arrange
            UpdateUserAccessGroup requestModel = new()
            {
                NewAccessGroupId = Guid.NewGuid().ToString(),
                OldAccessGroupId = Guid.NewGuid().ToString(),
                UserId = TestDataGenerator.Default_Id
            };

            AccessGroup newAccessGroup = TestDataGenerator.SecureAccessGroup;
            AccessGroup oldAccessGroup = TestDataGenerator.OpenAccessGroup;
            AppUser user = TestDataGenerator.AdminUser;

            _mockUnitOfWork.Setup(x => x.AccessGroups.GetAccessGroupByIdAsync(requestModel.OldAccessGroupId)).ReturnsAsync(oldAccessGroup);
            _mockUnitOfWork.Setup(x => x.AccessGroups.GetAccessGroupByIdAsync(requestModel.NewAccessGroupId)).ReturnsAsync(newAccessGroup);
            _mockUserManager.Setup(x => x.FindByIdAsync(requestModel.UserId)).ReturnsAsync(user);

            // Act
            var response = await _sut.UpdateUserAccessGroup(requestModel, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
            response.Data.ShouldBe(null);
            response.Message.ShouldBe(Constants.Generic_Fail_User_Does_Not_Belong_Message);
        }

        [Fact]
        public async Task UpdateUserAccessGroup_ShouldReturnOkResponse_WhenUpdateIsSuccessful()
        {
            // Arrange
            UpdateUserAccessGroup requestModel = new()
            {
                NewAccessGroupId = Guid.NewGuid().ToString(),
                OldAccessGroupId = Guid.NewGuid().ToString(),
                UserId = TestDataGenerator.Default_Id
            };

            AccessGroup newAccessGroup = TestDataGenerator.SecureAccessGroup;
            AccessGroup oldAccessGroup = TestDataGenerator.OpenAccessGroup;
            AppUser user = TestDataGenerator.AdminUser;

            oldAccessGroup.Users.Add(user);

            _mockUnitOfWork.Setup(x => x.AccessGroups.GetAccessGroupByIdAsync(requestModel.OldAccessGroupId)).ReturnsAsync(oldAccessGroup);
            _mockUnitOfWork.Setup(x => x.AccessGroups.GetAccessGroupByIdAsync(requestModel.NewAccessGroupId)).ReturnsAsync(newAccessGroup);
            _mockUserManager.Setup(x => x.FindByIdAsync(requestModel.UserId)).ReturnsAsync(user);
           
            // Act
            var response = await _sut.UpdateUserAccessGroup(requestModel, TestDataGenerator.SuperAdminUser.Id);

            // Assert
            response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            response.Succeeded.ShouldBe(true);
            response.Data.ShouldNotBe(null);
            response.Message.ShouldBe(ApiResponseMesage.User_Group_Update_Success);
        }
    }
}
