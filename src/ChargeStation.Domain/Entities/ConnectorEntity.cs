using ChargeStation.Domain.Common;
using System.Collections.Generic;

namespace ChargeStation.Domain.Entities
{
    public class ConnectorEntity : BaseEntity, IHasDomainEvent
    {
        public ConnectorEntity() { }

        public int AmpsMaxCurrent { get; set; }

        public int ChargeStationId { get; set; }

        public ChargeStationEntity ChargeStation { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
