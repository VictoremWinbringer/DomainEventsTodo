using DomainEventsTodo.Domain.Events;

namespace DomainEventsTodo.Dispatchers.Abstract
{
    public interface IDispatcher
    {
        void Dispatch(BaseEvent domainEvent);
    }
}