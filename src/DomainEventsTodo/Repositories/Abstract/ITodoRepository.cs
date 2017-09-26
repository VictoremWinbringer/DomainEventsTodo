using System;
using System.Collections.Generic;
using DomainEventsTodo.Domain;

namespace DomainEventsTodo.Repositories.Abstract
{
    public interface ITodoRepository : IEnumerable<Todo>
    {
        Todo this[Guid id] { get; }
        void Remove(Guid id);
        void Replace(Todo todo);
        void Add(Todo todo);
    }
}
