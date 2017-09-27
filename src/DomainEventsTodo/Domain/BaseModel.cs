using System;
using System.Collections.Generic;
using DomainEventsTodo.Domain.Events;
using DomainEventsTodo.Domain.Mementos;

namespace DomainEventsTodo.Domain
{
    public abstract class BaseModel<T> where T : BaseMemento
    {
        public Guid Id { get; protected set; }

        public abstract T Memento { get; }

        protected List<BaseEvent> InternalEvents { get; } = new List<BaseEvent>();

        public IReadOnlyList<BaseEvent> Events => InternalEvents.AsReadOnly();
    }
}