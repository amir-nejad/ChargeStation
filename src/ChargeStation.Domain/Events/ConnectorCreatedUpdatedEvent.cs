using ChargeStation.Domain.Common;
using ChargeStation.Domain.Entities;

namespace ChargeStation.Domain.Events
{
    public class ConnectorCreatedUpdatedEvent : DomainEvent
    {
        public ConnectorCreatedUpdatedEvent(ConnectorEntity connector) { 
            Connector = connector;
        }

        public ConnectorEntity Connector { get; set; }
    }
}
