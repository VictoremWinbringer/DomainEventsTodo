using System;
using System.Collections.Generic;
using DomainEventsTodo.Domain;

namespace DomainEventsTodo.Repositories.Abstract
{
    public interface ITodoRepository
    {
        Todo this[Guid id] { get; }
        IEnumerable<Todo> All();
        void Remove(Guid id);
        void Replace(Todo todo);
        void Add(Todo todo);
    }
}
