using DomainEventsTodo.Domain.Events;

namespace DomainEventsTodo.Dispatchers
{
    public interface IDispatcher
    {
        void Dispatch(BaseEvent domainEvent);
    }
}