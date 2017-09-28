using System;
using DomainEventsTodo.Domain;

namespace DomainEventsTodo.ViewModels
{
    public class TodoDisplayVm
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }

        public static TodoDisplayVm FromTodo(Todo todo)
        {
            var memento = todo.Memento;
            return new TodoDisplayVm
            {
                Id = memento.Id,
                Description = memento.Description,
                IsComplete = memento.IsComplete
            };
        }
    }
}
