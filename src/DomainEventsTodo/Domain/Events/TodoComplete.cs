using DomainEventsTodo.Domain.Mementos;

namespace DomainEventsTodo.Domain.Events
{
    public class TodoComplete : BaseEvent
    {
        public TodoComplete(TodoMemento memento)
        {
            this.Memento = memento;
        }
        public TodoMemento Memento { get; }
    }
}