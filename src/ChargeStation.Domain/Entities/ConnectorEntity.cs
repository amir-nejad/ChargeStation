using ChargeStation.Domain.Common;

namespace ChargeStation.Domain.Entities
{
    public class ConnectorEntity : BaseEntity
    {
        public ConnectorEntity() { }

        public int AmpsMaxCurrent { get; set; }

        public int ChargeStationId { get; set; }

        public ChargeStationEntity ChargeStation { get; set; }
    }
}
