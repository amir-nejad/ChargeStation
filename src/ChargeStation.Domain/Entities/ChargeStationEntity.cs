using ChargeStation.Domain.Common;
using System.Collections.Generic;

namespace ChargeStation.Domain.Entities
{
    public class ChargeStationEntity : BaseEntity
    {
        public ChargeStationEntity() { }

        public string Name { get; set; }

        public int GroupId { get; set; }

        public GroupEntity Group { get; set; }

        public IList<ConnectorEntity> Connectors { get; set; }
    }
}
