using ChargeStation.Application.Interfaces;
using ChargeStation.Application.Models;
using ChargeStation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChargeStation.Application.EventHandlers
{
    public class ConnectorCreatedEventHandler : INotificationHandler<DomainEventNotification<ConnectorCreatedUpdatedEvent>>
    {
        private readonly ILogger<ConnectorCreatedEventHandler> _logger;
        private readonly IChargeStationService _chargeStationService;
        private readonly IGroupService _groupService;

        public ConnectorCreatedEventHandler(ILogger<ConnectorCreatedEventHandler> logger, IChargeStationService chargeStationService, IGroupService groupService)
        {
            _logger = logger;
            _chargeStationService = chargeStationService;
            _groupService = groupService;
        }

        public async Task Handle(DomainEventNotification<ConnectorCreatedUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            // Getting the parent charge station
            var chargeStation = await _chargeStationService.GetChargeStationByIdAsync(domainEvent.Connector.ChargeStationId);

            if (chargeStation is null || chargeStation.Group is null)
                return;

            var group = chargeStation.Group;

            // Getting the sum of the all child connectors (that we know there is at least one child)
            var connectorsCapacity = chargeStation.Connectors.Sum(x => x.AmpsMaxCurrent);

            // Checking if we should increase the group capacity.
            if (group.AmpsCapacity < connectorsCapacity)
            {
                group.AmpsCapacity = connectorsCapacity;

                try
                {
                    await _groupService.UpdateGroupAsync(group);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}
