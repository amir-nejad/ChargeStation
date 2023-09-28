using ChargeStation.Application.Interfaces;
using ChargeStation.Domain.Entities;
using ChargeStation.WebApi.Controllers;
using ChargeStation.WebApi.Models.Dtos.ChargeStation;
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
    public class ChargeStationControllerTests
    {
        private Mock<IChargeStationService> _mockChargeStationService;
        private Mock<ILogger> _mockLogger;
        private ChargeStationController _chargeStationController;

        [SetUp]
        public void Setup()
        {
            _mockChargeStationService = new Mock<IChargeStationService>();
            _mockLogger = new Mock<ILogger>();
            _chargeStationController = new ChargeStationController(_mockChargeStationService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetChargeStationsAsync_ReturnsOkResult()
        {
            // Arrange
            var chargeStations = new List<ChargeStationEntity> { new ChargeStationEntity { Id = 1 } };
            _mockChargeStationService.Setup(service => service.GetChargeStationsAsync()).ReturnsAsync(chargeStations);

            // Act
            var result = await _chargeStationController.GetChargeStationsAsync();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<List<ChargeStationEntity>>(okResult.Value);
            var returnedChargeStations = okResult.Value as List<ChargeStationEntity>;
            Assert.AreEqual(chargeStations.Count, returnedChargeStations.Count);
        }

        [Test]
        public async Task GetChargeStationAsync_ValidId_ReturnsOkResult()
        {
            // Arrange
            var chargeStationId = 1;
            var chargeStationEntity = new ChargeStationEntity { Id = chargeStationId };
            _mockChargeStationService.Setup(service => service.GetChargeStationByIdAsync(chargeStationId)).ReturnsAsync(chargeStationEntity);

            // Act
            var result = await _chargeStationController.GetChargeStationAsync(chargeStationId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<ChargeStationDto>(okResult.Value);
            var returnedChargeStation = okResult.Value as ChargeStationDto;
            Assert.AreEqual(chargeStationId, returnedChargeStation.Id);
        }

        [Test]
        public async Task CreateChargeStationAsync_ValidChargeStation_ReturnsOkResult()
        {
            // Arrange
            var chargeStationDto = new ChargeStationDto { Id = 1, Name = "Test ChargeStation", CreatedDateUtc = DateTime.UtcNow, LastModifiedDateUtc = DateTime.UtcNow };
            var chargeStationEntity = new ChargeStationEntity { Id = chargeStationDto.Id.Value, Name = chargeStationDto.Name, CreatedDateUtc = chargeStationDto.CreatedDateUtc.Value, LastModifiedDateUtc = chargeStationDto.LastModifiedDateUtc.Value };
            _mockChargeStationService.Setup(service => service.CreateChargeStationAsync(It.IsAny<ChargeStationEntity>())).Returns(Task.CompletedTask);

            // Act
            var result = await _chargeStationController.CreateChargeStationAsync(chargeStationDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<CreateUpdateChargeStationResponseDto>(okResult.Value);
            var response = okResult.Value as CreateUpdateChargeStationResponseDto;
            Assert.True(response.Success);
            Assert.AreEqual(chargeStationDto.Id, response.ChargeStation.Id);
        }

        [Test]
        public async Task UpdateChargeStationAsync_ValidChargeStation_ReturnsOkResult()
        {
            // Arrange
            var chargeStationDto = new ChargeStationDto { Id = 1, Name = "Update Test ChargeStation", CreatedDateUtc = DateTime.UtcNow, LastModifiedDateUtc = DateTime.UtcNow };
            var chargeStationEntity = new ChargeStationEntity { Id = chargeStationDto.Id.Value, Name = chargeStationDto.Name, CreatedDateUtc = chargeStationDto.CreatedDateUtc.Value, LastModifiedDateUtc = chargeStationDto.LastModifiedDateUtc.Value };
            _mockChargeStationService.Setup(service => service.GetChargeStationByIdAsync(chargeStationDto.Id.Value)).ReturnsAsync(chargeStationEntity);
            _mockChargeStationService.Setup(service => service.UpdateChargeStationAsync(It.IsAny<ChargeStationEntity>())).Returns(Task.CompletedTask);

            // Act
            var result = await _chargeStationController.UpdateChargeStationAsync(chargeStationDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<CreateUpdateChargeStationResponseDto>(okResult.Value);
            var response = okResult.Value as CreateUpdateChargeStationResponseDto;
            Assert.True(response.Success);
            Assert.AreEqual(chargeStationDto.Id, response.ChargeStation.Id);
        }

        [Test]
        public async Task DeleteChargeStationAsync_ValidId_ReturnsOkResult()
        {
            // Arrange
            var chargeStationId = 1;
            _mockChargeStationService.Setup(service => service.DeleteChargeStationAsync(chargeStationId)).Returns(Task.CompletedTask);

            // Act
            var result = await _chargeStationController.DeleteChargeStationAsync(chargeStationId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<DeleteChargeStationResponseDto>(okResult.Value);
            var response = okResult.Value as DeleteChargeStationResponseDto;
            Assert.True(response.Success);
        }
    }
}
