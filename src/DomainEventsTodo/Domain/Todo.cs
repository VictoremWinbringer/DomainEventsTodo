using System;
using System.Collections.Generic;

namespace DomainEventsTodo.Domain
{
    public abstract class BaseEvent
    {
        public DateTime DateOccurred { get; } = DateTime.UtcNow;
    }

    public class TodoComplete : BaseEvent
    {
        public TodoComplete(string description)
        {
            this.Description = description;
        }
        public string Description { get; }
    }

    public abstract class BaseModel<T> where T : BaseMemento
    {
        protected Guid Id { get; set; }

        public abstract T Memento { get; }

        protected List<BaseEvent> InternalEvents { get; } = new List<BaseEvent>();

        public IReadOnlyList<BaseEvent> Events => InternalEvents.AsReadOnly();
    }
    public abstract class BaseMemento
    {
        public Guid Id { get; set; }
    }

    public class TodoMemento : BaseMemento
    {
        public string Description { get; set; }
        public bool IsComplete { get; set; } = false;
    }
    public class Todo : BaseModel<TodoMemento>
    {
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

        private string Description { get; set; }
        private bool IsComplete { get; set; }

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

            InternalEvents.Add(new TodoComplete(this.Description));
        }
    }
}
