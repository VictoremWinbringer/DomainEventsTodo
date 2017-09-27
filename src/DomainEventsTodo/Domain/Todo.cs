using System;
using DomainEventsTodo.Domain.Events;
using DomainEventsTodo.Domain.Mementos;

namespace DomainEventsTodo.Domain
{
    public class Todo : BaseModel<TodoMemento>
    {
        private string Description { get; set; }
        private bool IsComplete { get; set; }

        public Todo(string description)
        {
            this.Id = Guid.NewGuid();
            this.Description = description;
        }

        public Todo(TodoMemento memento)
        {
            this.Id = memento.Id;
            this.Description = memento.Description;
            this.IsComplete = memento.IsComplete;
        }

        public override TodoMemento Memento => new TodoMemento
        {
            Id = this.Id,
            Description = this.Description,
            IsComplete = this.IsComplete
        };

        public void Update(string description)
        {
            this.Description = description;
        }

        public void MakeComplete()
        {
            this.IsComplete = true;

            InternalEvents.Add(new TodoComplete(this.Memento));
        }
    }
}
