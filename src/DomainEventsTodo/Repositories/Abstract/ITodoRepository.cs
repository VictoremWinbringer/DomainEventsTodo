using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainEventsTodo.Domain;

namespace DomainEventsTodo.Repositories.Abstract
{
    public interface ITodoRepository : IEnumerable<Todo>
    {
        Todo this[Guid id] { get; set; }
        void Remove(Guid id);
    }
}
