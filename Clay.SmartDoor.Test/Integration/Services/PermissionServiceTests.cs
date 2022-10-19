using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Helpers;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
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
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clay.SmartDoor.Test.Integration.Services
{
    public class PermissionServiceTests
    {
        private readonly IPermissionService _sut;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<ILogger> mockLogger = new();

        public PermissionServiceTests()
        {
            _mockUserManager = MockHelpers.MockUserManager<AppUser>(TestDataGenerator.DummyUsers);
            _mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>(TestDataGenerator.DummyRoles);
            _sut = new PermissionService(mockLogger.Object, _mockRoleManager.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task GetAsync_ShouldOkResponse_WhenRoleExists()
        {
            // Arrange
            var role = Roles.SuperAdmin.ToString();
            var roleId = TestDataGenerator.Default_Id;

            _mockRoleManager.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync(new IdentityRole(role));
            _mockRoleManager.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityRole>())).ReturnsAsync(new List<Claim> { new Claim(ClaimTypes.Name, "Permission"), });

            // Act
            var response = await _sut.GetAsync(roleId);

            // Assert
            response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            response.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync_ShouldHandleExceptionAndReturnFailedResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var data = new LoginRequest { Email = "superadminuser@email.com", Password = "Password@123" };
            var role = Roles.SuperAdmin.ToString();
            var roleId = TestDataGenerator.Default_Id;

            _mockRoleManager.Setup(x => x.FindByIdAsync(roleId)).Throws(new Exception());
            _mockRoleManager.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityRole>())).ReturnsAsync(new List<Claim> { new Claim(ClaimTypes.Name, "Permission"), });

            // Act
            var response = await _sut.GetAsync(roleId);

            // Assert
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
            response.Message.ShouldBe(Constants.Generic_Operation_Failed_Message);
        }


        [Fact]
        public async Task GetUserPermissionsAsync_ShouldOkResponse_WhenRoleExists()
        {
            // Arrange

            var userId = TestDataGenerator.Default_Id;
            var user = TestDataGenerator.BasicUser;

            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var response = await _sut.GetUserPermissionsAsync(userId);

            // Assert
            response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task GetUserPermissionsAsync_ShouldReturnFailedResponse_WhenUserNotFound()
        {
            // Arrange
            var userId = TestDataGenerator.Default_Id;
            AppUser user = null!;

            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var response = await _sut.GetUserPermissionsAsync(userId);

            // Assert
            response.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
            response.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task GetUserPermissionsAsync_ShouldHandleExceptionAndReturnFailedResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = TestDataGenerator.Default_Id;

            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).Throws(new Exception());

            // Act
            var response = await _sut.GetUserPermissionsAsync(userId);

            // Assert
            response.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            response.Succeeded.ShouldBe(false);
            response.Message.ShouldBe(Constants.Generic_Operation_Failed_Message);
        }

        [Fact]
        public async Task UpdatePermissionsForRoleAsync_ShouldOkResponse_WhenUpdateSucceeds()
        {
            // Arrange

            var roleId = TestDataGenerator.Default_Id;
            var user = TestDataGenerator.BasicUser;
            var role = Roles.SuperAdmin.ToString();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Permission") };

            _mockRoleManager.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync(new IdentityRole(role));
            _mockRoleManager.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityRole>())).ReturnsAsync(claims);
            foreach (var claim in claims)
            {
                _mockRoleManager.Setup(x => x.RemoveClaimAsync(new IdentityRole(role), claim));

            }

            PermissionsDto permissionsDto = new()
            {
                RoleId = roleId,
                RoleClaims = new List<RoleClaimDto>()
                {
                    new RoleClaimDto(){Type = "Permission", Value = "Test", Selected = true}
                }

            };

            // Act
            var response = await _sut.UpdatePermissionsForRoleAsync(permissionsDto);

            // Assert
            response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            response.Succeeded.ShouldBe(true);
            response.Message.ShouldBe(Constants.Generic_Success_Message);
        }
    }
}
