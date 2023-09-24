using ChargeStation.Domain.Common;
using System.Collections.Generic;

namespace ChargeStation.Domain.Entities
{
    public class GroupEntity : BaseEntity
    {
        public GroupEntity() { }

        public string Name { get; set; }

        public int AmpsCapacity { get; set; }

        public IList<ChargeStationEntity> ChargeStations { get; set; }
    }
}
