using ChargeStation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChargeStation.Infrastructure.Persistance.Configurations
{
    public class ConnectorConfiguration : IEntityTypeConfiguration<ConnectorEntity>
    {
        public void Configure(EntityTypeBuilder<ConnectorEntity> builder)
        {
            builder.ToTable("Connectors");

            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.AmpsMaxCurrent)
                .IsRequired();

            builder.HasCheckConstraint("CK_Connectors_AmpsMaxCurrent", "AmpsMaxCurrent >= 0");
        }
    }
}
