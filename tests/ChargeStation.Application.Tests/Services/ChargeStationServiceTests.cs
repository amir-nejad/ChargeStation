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
using System.Linq;

namespace ChargeStation.UnitTests.Services
{
    [TestFixture]
    public class ChargeStationServiceTests
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
        public async Task CreateChargeStationAsync_ValidChargeStation_CallsEfRepositoryAddAsync()
        {
            // Arrange
            var chargeStation = new ChargeStationEntity();

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                var repository = new EfRepository<ChargeStationEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var chargeStationService = new ChargeStationService(repository, mockLogger.Object);

                // Act
                await chargeStationService.CreateChargeStationAsync(chargeStation);

                // Assert
                Assert.Contains(chargeStation, await dbContext.ChargeStations.ToListAsync());
            }
        }

        [Test]
        public async Task GetChargeStationByIdAsync_ValidId_ReturnsCorrectChargeStation()
        {
            // Arrange
            int chargeStationId = 1;
            var chargeStation = new ChargeStationEntity { Id = chargeStationId, GroupId = 1 };

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.Groups.Add(new GroupEntity()
                {
                    Id = 1,
                    Name = "Test Group"
                });
                await dbContext.SaveChangesAsync();

                dbContext.ChargeStations.Add(chargeStation);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<ChargeStationEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var chargeStationService = new ChargeStationService(repository, mockLogger.Object);

                // Act
                var result = await chargeStationService.GetChargeStationByIdAsync(chargeStationId);

                // Assert
                Assert.AreEqual(chargeStationId, result.Id);
            }
        }

        [Test]
        public async Task GetChargeStationsAsync_ReturnsAllChargeStations()
        {
            // Arrange
            var chargeStations = new List<ChargeStationEntity>
        {
            new ChargeStationEntity { Id = 1, Name = "Test 1", GroupId = 1 },
            new ChargeStationEntity { Id = 2, Name = "Test 2", GroupId = 1 },
            new ChargeStationEntity { Id = 3, Name = "Test 3", GroupId = 1 }
        };

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.Groups.Add(new GroupEntity()
                {
                    Id = 1,
                    Name = "Test Group"
                });
                await dbContext.SaveChangesAsync();

                dbContext.ChargeStations.AddRange(chargeStations);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<ChargeStationEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var chargeStationService = new ChargeStationService(repository, mockLogger.Object);

                // Act
                var result = await chargeStationService.GetChargeStationsAsync();

                // Assert
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(ChargeStationEntity));
            }
        }

        [Test]
        public async Task UpdateChargeStationAsync_ValidChargeStation_CallsEfRepositoryUpdateAsync()
        {
            // Arrange
            var chargeStation = new ChargeStationEntity();

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.ChargeStations.Add(chargeStation);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<ChargeStationEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var chargeStationService = new ChargeStationService(repository, mockLogger.Object);

                // Act
                await chargeStationService.UpdateChargeStationAsync(chargeStation);

                // Assert
                Assert.Contains(chargeStation, await dbContext.ChargeStations.ToListAsync());
                dbContext.Entry(chargeStation).State = EntityState.Detached; // Ensure it's detached for verification
            }
        }

        [Test]
        public async Task DeleteChargeStationAsync_ValidId_DeletesChargeStation()
        {
            // Arrange
            int chargeStationId = 1;
            var chargeStation = new ChargeStationEntity { Id = chargeStationId, GroupId = 1 };

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.Groups.Add(new GroupEntity()
                {
                    Id = 1,
                    Name = "Test Group"
                });
                await dbContext.SaveChangesAsync();

                dbContext.ChargeStations.Add(chargeStation);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<ChargeStationEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var chargeStationService = new ChargeStationService(repository, mockLogger.Object);

                // Act
                await chargeStationService.DeleteChargeStationAsync(chargeStationId);

                // Assert
                Assert.IsEmpty(dbContext.ChargeStations);
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