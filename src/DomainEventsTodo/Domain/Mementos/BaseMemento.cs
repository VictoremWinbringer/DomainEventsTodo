using System;

namespace DomainEventsTodo.Domain.Mementos
{
    public abstract class BaseMemento
    {
        public Guid Id { get; set; }
    }
}