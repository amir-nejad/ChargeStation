using ChargeStation.Application.Interfaces;
using ChargeStation.Application.Services;
using ChargeStation.Domain.Entities;
using ChargeStation.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Serilog;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using ChargeStation.Infrastructure.Services;
using MediatR;

namespace ChargeStation.UnitTests.Services
{
    [TestFixture]
    public class ConnectorServiceTests
    {
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;

        [SetUp]
        public void Setup()
        {
            // Set up an in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test
                .Options;
        }

        [Test]
        public async Task CreateConnectorAsync_ValidConnector_CallsEfRepositoryAddAsync()
        {
            // Arrange
            var connector = new ConnectorEntity();

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                var repository = new EfRepository<ConnectorEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var connectorService = new ConnectorService(repository, mockLogger.Object);

                // Act
                await connectorService.CreateConnectorAsync(connector);

                // Assert
                Assert.Contains(connector, await dbContext.Connectors.ToListAsync());
            }
        }

        [Test]
        public async Task GetConnectorByIdAsync_ValidId_ReturnsCorrectConnector()
        {
            // Arrange
            int connectorId = 1;
            var connector = new ConnectorEntity { Id = connectorId, ChargeStationId = 1 };

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.ChargeStations.Add(new ChargeStationEntity()
                {
                    Id = 1,
                    Name = "Test ChargeStation"
                });
                await dbContext.SaveChangesAsync();

                dbContext.Connectors.Add(connector);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<ConnectorEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var connectorService = new ConnectorService(repository, mockLogger.Object);

                // Act
                var result = await connectorService.GetConnectorByIdAsync(connectorId);

                // Assert
                Assert.AreEqual(connectorId, result.Id);
            }
        }

        [Test]
        public async Task GetConnectorsAsync_ReturnsAllConnectors()
        {
            // Arrange
            var connectors = new List<ConnectorEntity>
        {
            new ConnectorEntity { Id = 1, AmpsMaxCurrent = 100, ChargeStationId = 1 },
            new ConnectorEntity { Id = 2, AmpsMaxCurrent = 120, ChargeStationId = 1 },
            new ConnectorEntity { Id = 3, AmpsMaxCurrent = 130, ChargeStationId = 1 }
        };

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.ChargeStations.Add(new ChargeStationEntity()
                {
                    Id = 1,
                    Name = "Test ChargeStation"
                });
                await dbContext.SaveChangesAsync();

                dbContext.Connectors.AddRange(connectors);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<ConnectorEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var connectorService = new ConnectorService(repository, mockLogger.Object);

                // Act
                var result = await connectorService.GetConnectorsAsync();

                // Assert
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ConnectorEntity));
            }
        }

        [Test]
        public async Task UpdateConnectorAsync_ValidConnector_CallsEfRepositoryUpdateAsync()
        {
            // Arrange
            var connector = new ConnectorEntity();

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.Connectors.Add(connector);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<ConnectorEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var connectorService = new ConnectorService(repository, mockLogger.Object);

                // Act
                await connectorService.UpdateConnectorAsync(connector);

                // Assert
                Assert.Contains(connector, await dbContext.Connectors.ToListAsync());
                dbContext.Entry(connector).State = EntityState.Detached; // Ensure it's detached for verification
            }
        }

        [Test]
        public async Task DeleteConnectorAsync_ValidId_DeletesConnector()
        {
            // Arrange
            int connectorId = 1;
            var connector = new ConnectorEntity { Id = connectorId, ChargeStationId = 1 };

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.ChargeStations.Add(new ChargeStationEntity()
                {
                    Id = 1,
                    Name = "Test ChargeStation"
                });
                await dbContext.SaveChangesAsync();

                dbContext.Connectors.Add(connector);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<ConnectorEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var connectorService = new ConnectorService(repository, mockLogger.Object);

                // Act
                await connectorService.DeleteConnectorAsync(connectorId);

                // Assert
                Assert.IsEmpty(dbContext.Connectors);
            }
        }

        private IDomainEventService InitializeDomainEventService()
        {
            var domainServiceMockLogger = new Mock<ILogger>();
            var mockMediatorPublisher = new Mock<IPublisher>();
            var domainEventService = new DomainEventService(domainServiceMockLogger.Object, mockMediatorPublisher.Object);

            return domainEventService;
        }
    }
}