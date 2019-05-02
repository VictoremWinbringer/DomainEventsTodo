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
            return new TodoDisplayVm
            {
                Id = todo.Id,
                Description = todo.Description,
                IsComplete = todo.IsComplete
            };
        }
    }
}
