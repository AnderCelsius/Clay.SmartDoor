using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.CoreServices;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Core.Models.Constants;
using Clay.SmartDoor.Core.Services;
using Clay.SmartDoor.Test.Helper;
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
    public class DoorServiceTests
    {
        IDoorService _sut;
        private readonly Mock<ILogger> mockLogger = new();
        private readonly Mock<IDoorRepository> mockDoorRepository = new();
        private readonly Mock<IActivityLogRepository> mockActivityLogRepository = new();
        private readonly Mock<IUnitOfWork> mockUnitOfWork = new();
        public DoorServiceTests()
        {
            _sut = new DoorService(mockUnitOfWork.Object, mockLogger.Object);
            mockUnitOfWork.Setup(uow => uow.Doors).Returns(mockDoorRepository.Object);
            mockUnitOfWork.Setup(uow => uow.ActivityLogs).Returns(mockActivityLogRepository.Object);
        }

        [Fact]
        public async Task CreateNewDoorAsync_ShouldReturnCreatedResponseWithDoorId_WhenCreatedSuccesfuly()
        {
            // Arrange
            var creatorId = TestDataGenerator.Default_Id;
            var requestModel = new CreateDoorRecord("Main Door", "Uno", "1st Floor");

            var door = requestModel.ToDoor(DateTime.Now, DateTime.Now, creatorId);
            var activityLog = new ActivityLog
            {
                Time = DateTime.Now,
                Description = ActivityDescriptions.Door_Created,
                ActionBy = creatorId,
                DoorId = door.Id,
                Building = requestModel.Building,
                Floor = requestModel.Floor,
                DoorTag = door.NameTag,
            };

            mockUnitOfWork.Setup(t => t.Doors.AddAsync(door));
            mockUnitOfWork.Setup(t => t.ActivityLogs.AddAsync(activityLog));

            // Act

            var result = await _sut.CreateNewDoorAsync(requestModel, creatorId);

            // Assert
            result.StatusCode.ShouldBe(201);
            result.Message.ShouldBe(ApiResponseMesage.Created_Successfully);
            result.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task CreateNewDoorAsync_ShouldHandleExceptionAndReturnFailedResponse_WhenAnyFailsToAdd()
        {
            // Arrange
            mockUnitOfWork.Setup(t => t.Doors.AddAsync(It.IsAny<Door>())).ThrowsAsync(new Exception());
            mockUnitOfWork.Setup(t => t.ActivityLogs.AddAsync(It.IsAny<ActivityLog>())).ThrowsAsync(new Exception());

            // Act

            var result = await _sut.CreateNewDoorAsync(TestDataGenerator.RequestModel, TestDataGenerator.ActionBy);

            // Assert
            result.StatusCode.ShouldBe(400);
            result.Message.ShouldBe(ApiResponseMesage.Failed_To_Create);
            result.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task ExitDoorAsync_ShouldReturnSuccessResponse_WhenDoorExists()
        {
            // Arrange
            mockUnitOfWork.Setup(uow => uow.Doors.GetDoorAsync(It.IsAny<string>())).ReturnsAsync(TestDataGenerator.DefaultDoor);

            // Act
            var result = await _sut.ExitDoorAsync(TestDataGenerator.Default_Id, TestDataGenerator.ActionBy);

            // Assert
            result.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            result.Message.ShouldBe(ApiResponseMesage.Ok_Result);
            result.Succeeded.ShouldBe(true);
        }

        [Fact]
        public async Task ExitDoorAsync_ShouldReturnFailedResponse_WhenDoorDoesNotExist()
        {
            // Arrange
            Door door = null!;
            mockUnitOfWork.Setup(uow => uow.Doors.GetDoorAsync(It.IsAny<string>())).ReturnsAsync(door);

            // Act
            var result = await _sut.ExitDoorAsync(TestDataGenerator.Default_Id, TestDataGenerator.ActionBy);

            // Assert
            result.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
            result.Message.ShouldBe(DoorMessage.Not_Found);
            result.Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task GetDoorsAsync_ShouldOkResponse_WhenNoDoorExist()
        {
            // Arrange
            List<Door> doors = new();
            mockUnitOfWork.Setup(uow => uow.Doors.GetAllDoorsAsync()).Returns(doors.AsQueryable());

            // Act
            var result = await _sut.GetDoorsAsync();

            // Assert
            result.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            result.Message.ShouldBe(ApiResponseMesage.Ok_Result);
            result.Data.Count().ShouldBe(0);
            result.Succeeded.ShouldBe(true);
        }

    }
}
