using Clay.SmartDoor.Core.DTOs.Authentication;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Models.Constants;
using Clay.SmartDoor.Core.Models.Enums;
using Clay.SmartDoor.Core.Services;
using Clay.SmartDoor.Test.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Serilog;
using Shouldly;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Clay.SmartDoor.Test.Integration.Services
{
    public class AuthenticationServiceTests
    {
        IAuthenticationService _sut;

        private readonly Mock<ILogger> mockLogger = new();
        private readonly Mock<IConfiguration> mockConfiguration = new();
        private readonly Mock<IConfigurationSection> mockConfigurationSection = new();
        private readonly Mock<UserManager<AppUser>> _userManager;
        private readonly Mock<RoleManager<IdentityRole>> _roleManager;
        public AuthenticationServiceTests()
        {
            _userManager = MockHelpers.MockUserManager<AppUser>(TestDataGenerator.DummyUsers);
            _roleManager = MockHelpers.MockRoleManager<IdentityRole>(TestDataGenerator.DummyRoles);
            _sut = new AuthenticationService(
                        mockLogger.Object, _userManager.Object,
                        _roleManager.Object, mockConfiguration.Object);
        }


        [Fact]
        public async Task AuthenticateUserAsync_ShouldUnauthorizedResponse_WhenUserDoesNotExist()
        {
            // Arrange
            var data = new LoginRequest { Email = "fakeuser@email.com", Password = "Password@123" };

            // Act
            var response = await _sut.AuthenticateUserAsync(data);

            // Assert
            response.Message.ShouldBe(AuthenticationMessage.Invalid_Credentials);
            response.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldOkResponse_WhenUserExistAnIsActive()
        {
            // Arrange
            var data = new LoginRequest { Email = "superadminuser@email.com", Password = "Password@123" };
            var role = Roles.SuperAdmin.ToString();
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(TestDataGenerator.SuperAdminUser);
            _userManager.Setup(x => x.CheckPasswordAsync(TestDataGenerator.SuperAdminUser, data.Password)).ReturnsAsync(true);
            _userManager.Setup(x => x.IsEmailConfirmedAsync(TestDataGenerator.SuperAdminUser)).ReturnsAsync(true);
            _userManager.Setup(x => x.GetRolesAsync(TestDataGenerator.SuperAdminUser)).ReturnsAsync(new List<string> { role });

            _roleManager.Setup(x => x.FindByNameAsync(Roles.SuperAdmin.ToString())).ReturnsAsync(new IdentityRole(role));
            _roleManager.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityRole>())).ReturnsAsync(new List<Claim> { new Claim(ClaimTypes.Name, "Permission"),});

            mockConfigurationSection.Setup(x => x.Value).Returns("XYZk7Q1234g5p6DogCatL0MHappySadcOj6UpDown123");
            mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
            mockConfiguration.Setup(x => x.GetSection("Key")).Returns(mockConfigurationSection.Object);

            // Act
            var response = await _sut.AuthenticateUserAsync(data);

            // Assert
            response.Message.ShouldBe(AuthenticationMessage.Login_Success);
            response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldForbiddenResponse_WhenUserExistButInActive()
        {
            // Arrange
            var data = new LoginRequest { Email = "test@email.com", Password = "Password@123" };
            var role = Roles.SuperAdmin.ToString();
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(TestDataGenerator.InActiveUser);
            _userManager.Setup(x => x.CheckPasswordAsync(TestDataGenerator.InActiveUser, data.Password)).ReturnsAsync(true);
            _userManager.Setup(x => x.IsEmailConfirmedAsync(TestDataGenerator.InActiveUser)).ReturnsAsync(true);

            // Act
            var response = await _sut.AuthenticateUserAsync(data);

            // Assert
            response.Message.ShouldBe(AuthenticationMessage.Not_Activated);
            response.StatusCode.ShouldBe((int)HttpStatusCode.Forbidden);
        }

    }
}
