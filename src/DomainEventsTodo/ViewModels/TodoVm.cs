using System;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Description { get; set; }
    }
}
