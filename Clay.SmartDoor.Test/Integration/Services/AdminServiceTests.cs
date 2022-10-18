using Clay.SmartDoor.Core.DTOs.Admin;
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
        private readonly Mock<ILogger> _mockLogger = new();
        public AdminServiceTests()
        {
            _mockUserManager = MockHelpers.MockUserManager<AppUser>(TestDataGenerator.DummyUsers);
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(x => x.ActivityLogs).Returns(_mockActivityLogRepo.Object);
            _sut = new AdminService(_mockUserManager.Object, _mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddAccessGroupAsync_ShouldReturnFailResponse_WhenGroupNameExist()
        {
            // Arrange
            var groupName = "Test";
            var accessGroup = new AccessGroup
            {
                Name = groupName,
                IsActive = true,
                Users = TestDataGenerator.DummyUsers
            };
            _mockUnitOfWork.Setup(uow => uow.AccessGroups.GetAccessGroupsAsync(groupName)).ReturnsAsync(accessGroup);

            // Act
            var response = await _sut.AddAccessGroupAsync(groupName, TestDataGenerator.SuperAdminUser.Id);

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
            var accessGroup = new AccessGroup
            {
                Name = groupName,
                IsActive = true,
                Users = TestDataGenerator.DummyUsers
            };

            AccessGroup noAccessGroup = null!;
            _mockUnitOfWork.Setup(uow => uow.AccessGroups.GetAccessGroupsAsync(groupName)).ReturnsAsync(noAccessGroup);

            // Act
            var response = await _sut.AddAccessGroupAsync(groupName, TestDataGenerator.ActionBy);

            // Assert
            response.Message.ShouldBe(ApiResponseMesage.Created_Successfully);
            response.StatusCode.ShouldBe((int)HttpStatusCode.Created);
            response.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task AddAccessGroupAsync_ShouldHandleExceptionAndReturnfailedResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var groupName = "Test";
            var accessGroup = new AccessGroup
            {
                Name = groupName,
                IsActive = true,
                Users = TestDataGenerator.DummyUsers
            };

            AccessGroup noAccessGroup = null!;
            _mockUnitOfWork.Setup(uow => uow.AccessGroups.GetAccessGroupsAsync(groupName)).ReturnsAsync(noAccessGroup);
            _mockUnitOfWork.Setup(uow => uow.ActivityLogs.AddAsync(It.IsAny<ActivityLog>())).Throws(new Exception());

            // Act
            var response = await _sut.AddAccessGroupAsync(groupName, TestDataGenerator.ActionBy);

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
                GroupId = TestDataGenerator.Default_AccessGroup

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
                GroupId = TestDataGenerator.Default_AccessGroup

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
        public async Task AddUserAsync_ShouldHandleExceptionAndReturnfailedResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var data = new NewUserRequest
            {
                FirstName = TestDataGenerator.BasicUser.FirstName,
                LastName = TestDataGenerator.BasicUser.LastName,
                Email = TestDataGenerator.BasicUser.Email,
                Password = "Password@123",
                GroupId = TestDataGenerator.Default_AccessGroup

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
        public async Task GetAllAccessGroupsAsync_ShouldHandleExceptionAndReturnfailedResponse_WhenExceptionIsThrown()
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
    }
}
