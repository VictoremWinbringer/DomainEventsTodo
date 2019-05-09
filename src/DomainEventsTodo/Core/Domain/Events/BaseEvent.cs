using System;

namespace DomainEventsTodo.Domain.Events
{
    public abstract class BaseEvent
    {
        public DateTime DateOccurred { get; } = DateTime.UtcNow;

    }
}
