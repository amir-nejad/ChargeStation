using ChargeStation.Application.Interfaces;
using ChargeStation.Application.Models;
using ChargeStation.Domain.Common;
using MediatR;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ChargeStation.Infrastructure.Services
{
    public class DomainEventService : IDomainEventService
    {
        private readonly ILogger _logger;
        private readonly IPublisher _mediator;

        public DomainEventService(ILogger logger, IPublisher mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Publish(DomainEvent domainEvent)
        {
            _logger.Information("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
            await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
        }

        private INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
        {
            return (INotification)Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);
        }
    }
}
