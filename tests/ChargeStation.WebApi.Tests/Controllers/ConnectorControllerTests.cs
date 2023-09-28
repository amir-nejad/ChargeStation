using ChargeStation.Application.Interfaces;
using ChargeStation.Domain.Entities;
using ChargeStation.WebApi.Controllers;
using ChargeStation.WebApi.Models.Dtos.Connector;
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
    public class ConnectorControllerTests
    {
        private Mock<IConnectorService> _mockConnectorService;
        private Mock<ILogger> _mockLogger;
        private Mock<IChargeStationService> _mockChargeStationService;
        private ConnectorController _connectorController;

        [SetUp]
        public void Setup()
        {
            _mockConnectorService = new Mock<IConnectorService>();
            _mockLogger = new Mock<ILogger>();
            _mockChargeStationService = new Mock<IChargeStationService>();
            _connectorController = new ConnectorController(_mockConnectorService.Object, _mockLogger.Object, _mockChargeStationService.Object);
        }

        [Test]
        public async Task GetConnectorsAsync_ReturnsOkResult()
        {
            // Arrange
            var connectors = new List<ConnectorEntity> { new ConnectorEntity { Id = 1 } };
            _mockConnectorService.Setup(service => service.GetConnectorsAsync()).ReturnsAsync(connectors);

            // Act
            var result = await _connectorController.GetConnectorsAsync();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<List<ConnectorEntity>>(okResult.Value);
            var returnedConnectors = okResult.Value as List<ConnectorEntity>;
            Assert.AreEqual(connectors.Count, returnedConnectors.Count);
        }

        [Test]
        public async Task GetConnectorAsync_ValidId_ReturnsOkResult()
        {
            // Arrange
            var connectorId = 1;
            var connectorEntity = new ConnectorEntity { Id = connectorId };
            _mockConnectorService.Setup(service => service.GetConnectorByIdAsync(connectorId)).ReturnsAsync(connectorEntity);

            // Act
            var result = await _connectorController.GetConnectorAsync(connectorId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<ConnectorDto>(okResult.Value);
            var returnedConnector = okResult.Value as ConnectorDto;
            Assert.AreEqual(connectorId, returnedConnector.Id);
        }

        [Test]
        public async Task CreateConnectorAsync_ValidConnector_ReturnsOkResult()
        {
            // Arrange
            var connectorDto = new ConnectorDto { Id = 1, AmpsMaxCurrent = 10, ChargeStationId = 1, CreatedDateUtc = DateTime.UtcNow, LastModifiedDateUtc = DateTime.UtcNow };
            var connectorEntity = new ConnectorEntity { Id = connectorDto.Id.Value, AmpsMaxCurrent = connectorDto.AmpsMaxCurrent, ChargeStationId = connectorDto.ChargeStationId, CreatedDateUtc = connectorDto.CreatedDateUtc.Value, LastModifiedDateUtc = connectorDto.LastModifiedDateUtc.Value };
            _mockConnectorService.Setup(service => service.CreateConnectorAsync(It.IsAny<ConnectorEntity>())).Returns(Task.CompletedTask);
           
            _mockChargeStationService.Setup(service => 
                service.GetChargeStationByIdAsync(connectorDto.Id.Value)).Returns(Task.FromResult(new ChargeStationEntity()
                {
                    Id = connectorDto.ChargeStationId,
                    Name = "Test ChargeStation",
                    CreatedDateUtc = DateTime.UtcNow,
                    LastModifiedDateUtc = DateTime.UtcNow,
                    GroupId = 1,
                }));

            // Act
            var result = await _connectorController.CreateConnectorAsync(connectorDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<CreateUpdateConnectorResponseDto>(okResult.Value);
            var response = okResult.Value as CreateUpdateConnectorResponseDto;
            Assert.True(response.Success);
            Assert.AreEqual(connectorDto.Id, response.Connector.Id);
        }

        [Test]
        public async Task UpdateConnectorAsync_ValidConnector_ReturnsOkResult()
        {
            // Arrange
            var connectorDto = new ConnectorDto { Id = 1, AmpsMaxCurrent = 10, CreatedDateUtc = DateTime.UtcNow, LastModifiedDateUtc = DateTime.UtcNow };
            var connectorEntity = new ConnectorEntity { Id = connectorDto.Id.Value, AmpsMaxCurrent = connectorDto.AmpsMaxCurrent, CreatedDateUtc = connectorDto.CreatedDateUtc.Value, LastModifiedDateUtc = connectorDto.LastModifiedDateUtc.Value };
            _mockConnectorService.Setup(service => service.GetConnectorByIdAsync(connectorDto.Id.Value)).ReturnsAsync(connectorEntity);
            _mockConnectorService.Setup(service => service.UpdateConnectorAsync(It.IsAny<ConnectorEntity>())).Returns(Task.CompletedTask);

            // Act
            var result = await _connectorController.UpdateConnectorAsync(connectorDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<CreateUpdateConnectorResponseDto>(okResult.Value);
            var response = okResult.Value as CreateUpdateConnectorResponseDto;
            Assert.True(response.Success);
            Assert.AreEqual(connectorDto.Id, response.Connector.Id);
        }

        [Test]
        public async Task DeleteConnectorAsync_ValidId_ReturnsOkResult()
        {
            // Arrange
            var connectorId = 1;
            _mockConnectorService.Setup(service => service.DeleteConnectorAsync(connectorId)).Returns(Task.CompletedTask);

            // Act
            var result = await _connectorController.DeleteConnectorAsync(connectorId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<DeleteConnectorResponseDto>(okResult.Value);
            var response = okResult.Value as DeleteConnectorResponseDto;
            Assert.True(response.Success);
        }
    }
}
