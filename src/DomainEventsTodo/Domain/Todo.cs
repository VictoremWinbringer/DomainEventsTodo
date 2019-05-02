using System;
using System.Collections.Generic;
using DomainEventsTodo.Domain.Events;

namespace DomainEventsTodo.Domain
{
    public class Todo 
    {
        public Guid Id { get; private set; }

        private readonly List<BaseEvent> _events = new List<BaseEvent>();

        public IReadOnlyList<BaseEvent> Events => _events.AsReadOnly();
        public string Description { get; private set; }
        public bool IsComplete { get; private set; }

        public Todo(string description)
        {
            Id = Guid.NewGuid();
            Description = description;
        }

        public Todo(Guid id, string description, bool isComplete)
        {
            Id = id;
            Description = description;
            IsComplete = isComplete;
        }

        public void Update(string description)
        {
            Description = description;
        }

        public void MakeComplete()
        {
            IsComplete = true;

            _events.Add(new TodoComplete(Description));
        }
    }
}
