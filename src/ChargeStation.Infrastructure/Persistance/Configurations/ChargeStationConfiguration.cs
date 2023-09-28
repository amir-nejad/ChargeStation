using ChargeStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargeStation.Infrastructure.Persistance.Configurations
{
    public class ChargeStationConfiguration : IEntityTypeConfiguration<ChargeStationEntity>
    {
        public void Configure(EntityTypeBuilder<ChargeStationEntity> builder)
        {
            //builder.ToTable("ChargeStations");

            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .HasMany(x => x.Connectors)
                .WithOne(x => x.ChargeStation)
                .HasForeignKey(x => x.ChargeStationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
