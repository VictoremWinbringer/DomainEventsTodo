namespace DomainEventsTodo.Domain.Mementos
{
    public class TodoMemento : BaseMemento
    {
        public string Description { get; set; }
        public bool IsComplete { get; set; } = false;
    }
}