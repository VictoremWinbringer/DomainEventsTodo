using DomainEventsTodo.Domain.Events;

namespace DomainEventsTodo.Domain
{
    public interface IHandler<T> where T : BaseEvent
    {
        void Handle(T doaminEvent);
    }
}