using DomainEventsTodo.Domain.Events;

namespace DomainEventsTodo.Handlers
{
    public interface IHandler<T> where T : BaseEvent
    {
        void Handle(T doaminEvent);
    }
}