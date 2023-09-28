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
    public class GroupServiceTests
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
        public async Task CreateGroupAsync_ValidGroup_CallsEfRepositoryAddAsync()
        {
            // Arrange
            var group = new GroupEntity();

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                var repository = new EfRepository<GroupEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var groupService = new GroupService(repository, mockLogger.Object);

                // Act
                await groupService.CreateGroupAsync(group);

                // Assert
                Assert.Contains(group, await dbContext.Groups.ToListAsync());
            }
        }

        [Test]
        public async Task GetGroupByIdAsync_ValidId_ReturnsCorrectGroup()
        {
            // Arrange
            int groupId = 1;
            var group = new GroupEntity { Id = groupId };

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.Groups.Add(group);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<GroupEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var groupService = new GroupService(repository, mockLogger.Object);

                // Act
                var result = await groupService.GetGroupByIdAsync(groupId);

                // Assert
                Assert.AreEqual(groupId, result.Id);
            }
        }

        [Test]
        public async Task GetGroupsAsync_ReturnsAllGroups()
        {
            // Arrange
            var groups = new List<GroupEntity>
        {
            new GroupEntity { Id = 1, Name = "Test 1", AmpsCapacity = 10, ChargeStations = new List<ChargeStationEntity>() },
            new GroupEntity { Id = 2, Name = "Test 2", AmpsCapacity = 20, ChargeStations = new List<ChargeStationEntity>() },
            new GroupEntity { Id = 3, Name = "Test 3", AmpsCapacity = 30, ChargeStations = new List<ChargeStationEntity>() }
        };

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.Groups.AddRange(groups);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<GroupEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var groupService = new GroupService(repository, mockLogger.Object);

                // Act
                var result = await groupService.GetGroupsAsync();

                // Assert
                CollectionAssert.AllItemsAreInstancesOfType(result, typeof(GroupEntity));
            }
        }

        [Test]
        public async Task UpdateGroupAsync_ValidGroup_CallsEfRepositoryUpdateAsync()
        {
            // Arrange
            var group = new GroupEntity();

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.Groups.Add(group);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<GroupEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var groupService = new GroupService(repository, mockLogger.Object);

                // Act
                await groupService.UpdateGroupAsync(group);

                // Assert
                Assert.Contains(group, await dbContext.Groups.ToListAsync());
                dbContext.Entry(group).State = EntityState.Detached; // Ensure it's detached for verification
            }
        }

        [Test]
        public async Task DeleteGroupAsync_ValidId_DeletesGroup()
        {
            // Arrange
            int groupId = 1;
            var group = new GroupEntity { Id = groupId };

            var domainEventService = InitializeDomainEventService();

            using (var dbContext = new ApplicationDbContext(_dbContextOptions, domainEventService))
            {
                dbContext.Groups.Add(group);
                await dbContext.SaveChangesAsync();

                var repository = new EfRepository<GroupEntity>(dbContext);
                var mockLogger = new Mock<ILogger>();
                var groupService = new GroupService(repository, mockLogger.Object);

                // Act
                await groupService.DeleteGroupAsync(groupId);

                // Assert
                Assert.IsEmpty(dbContext.Groups);
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