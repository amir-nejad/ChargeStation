using ChargeStation.Domain.Common;
using System.Threading.Tasks;

namespace ChargeStation.Application.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
