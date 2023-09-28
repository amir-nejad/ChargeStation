using ChargeStation.Application.Interfaces;
using ChargeStation.Domain.Entities;
using ChargeStation.WebApi.Controllers;
using ChargeStation.WebApi.Models.Dtos.Group;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeStation.WebApi.Tests.Controllers
{
    [TestFixture]
    public class GroupControllerTests
    {
        private Mock<IGroupService> _mockGroupService;
        private Mock<ILogger> _mockLogger;
        private GroupController _groupController;

        [SetUp]
        public void Setup()
        {
            _mockGroupService = new Mock<IGroupService>();
            _mockLogger = new Mock<ILogger>();
            _groupController = new GroupController(_mockGroupService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetGroupsAsync_ReturnsOkResult()
        {
            // Arrange
            var groups = new List<GroupEntity> { new GroupEntity { Id = 1 } };
            _mockGroupService.Setup(service => service.GetGroupsAsync()).ReturnsAsync(groups);

            // Act
            var result = await _groupController.GetGroupsAsync();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<List<GroupEntity>>(okResult.Value);
            var returnedGroups = okResult.Value as List<GroupEntity>;
            Assert.AreEqual(groups.Count, returnedGroups.Count);
        }

        [Test]
        public async Task GetGroupAsync_ValidId_ReturnsOkResult()
        {
            // Arrange
            var groupId = 1;
            var groupEntity = new GroupEntity { Id = groupId };
            _mockGroupService.Setup(service => service.GetGroupByIdAsync(groupId)).ReturnsAsync(groupEntity);

            // Act
            var result = await _groupController.GetGroupAsync(groupId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<GroupDto>(okResult.Value);
            var returnedGroup = okResult.Value as GroupDto;
            Assert.AreEqual(groupId, returnedGroup.Id);
        }

        [Test]
        public async Task CreateGroupAsync_ValidGroup_ReturnsOkResult()
        {
            // Arrange
            var groupDto = new GroupDto { Id = 1, AmpsCapacity = 10, Name = "Test Group", CreatedDateUtc = DateTime.UtcNow, LastModifiedDateUtc = DateTime.UtcNow };
            var groupEntity = new GroupEntity { Id = groupDto.Id.Value, AmpsCapacity = groupDto.AmpsCapacity, Name = groupDto.Name, CreatedDateUtc = groupDto.CreatedDateUtc.Value, LastModifiedDateUtc = groupDto.LastModifiedDateUtc.Value };
            _mockGroupService.Setup(service => service.CreateGroupAsync(It.IsAny<GroupEntity>())).Returns(Task.CompletedTask);

            // Act
            var result = await _groupController.CreateGroupAsync(groupDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<CreateUpdateGroupResponseDto>(okResult.Value);
            var response = okResult.Value as CreateUpdateGroupResponseDto;
            Assert.True(response.Success);
            Assert.AreEqual(groupDto.Id, response.Group.Id);
        }

        [Test]
        public async Task UpdateGroupAsync_ValidGroup_ReturnsOkResult()
        {
            // Arrange
            var groupDto = new GroupDto { Id = 1, AmpsCapacity = 10, Name = "Update Test Group", CreatedDateUtc = DateTime.UtcNow, LastModifiedDateUtc = DateTime.UtcNow };
            var groupEntity = new GroupEntity { Id = groupDto.Id.Value, AmpsCapacity = groupDto.AmpsCapacity, Name = groupDto.Name, CreatedDateUtc = groupDto.CreatedDateUtc.Value, LastModifiedDateUtc = groupDto.LastModifiedDateUtc.Value };
            _mockGroupService.Setup(service => service.GetGroupByIdAsync(groupDto.Id.Value)).ReturnsAsync(groupEntity);
            _mockGroupService.Setup(service => service.UpdateGroupAsync(It.IsAny<GroupEntity>())).Returns(Task.CompletedTask);

            // Act
            var result = await _groupController.UpdateGroupAsync(groupDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<CreateUpdateGroupResponseDto>(okResult.Value);
            var response = okResult.Value as CreateUpdateGroupResponseDto;
            Assert.True(response.Success);
            Assert.AreEqual(groupDto.Id, response.Group.Id);
        }

        [Test]
        public async Task DeleteGroupAsync_ValidId_ReturnsOkResult()
        {
            // Arrange
            var groupId = 1;
            _mockGroupService.Setup(service => service.DeleteGroupAsync(groupId)).Returns(Task.CompletedTask);

            // Act
            var result = await _groupController.DeleteGroupAsync(groupId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<DeleteGroupResponseDto>(okResult.Value);
            var response = okResult.Value as DeleteGroupResponseDto;
            Assert.True(response.Success);
        }
    }
}
