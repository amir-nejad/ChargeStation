using ChargeStation.Application.Interfaces;
using ChargeStation.Domain.Common;
using ChargeStation.Domain.Entities;
using ChargeStation.Infrastructure.Persistance.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChargeStation.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDomainEventService _domainEventService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                    IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<ChargeStationEntity> ChargeStations { get; set; }
        public DbSet<ConnectorEntity> Connectors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ChargeStationConfiguration());
            modelBuilder.ApplyConfiguration(new ConnectorConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDateUtc = DateTime.UtcNow;
                        entry.Entity.LastModifiedDateUtc = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedDateUtc = DateTime.UtcNow;
                        break;
                }
            }

            var events = ChangeTracker.Entries<IHasDomainEvent>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .ToArray();

            var result = await base.SaveChangesAsync(cancellationToken);

            await DispatchEvents(events);

            return result;
        }

        private async Task DispatchEvents(DomainEvent[] events)
        {
            foreach (var @event in events)
            {
                @event.IsPublished = true;
                await _domainEventService.Publish(@event);
            }
        }
    }
}
