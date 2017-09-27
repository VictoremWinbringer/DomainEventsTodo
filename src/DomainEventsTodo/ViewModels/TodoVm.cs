using System;
using DomainEventsTodo.Domain;

namespace DomainEventsTodo.ViewModels
{
    public class TodoVm
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }

        public static TodoVm FromTodo(Todo todo)
        {
            var memento = todo.Memento;
            return new TodoVm
            {
                Id = memento.Id,
                Description = memento.Description,
                IsComplete = memento.IsComplete
            };
        }
    }

    public class TodoPostPutVm
    {
        public string Description { get; set; }
    }
}
