
namespace DomainEventsTodo.Domain.Events
{
    public class TodoComplete : BaseEvent
    {
        public TodoComplete(string description)
        {
            Description = description;
        }
        
        public string Description { get; }
    }
}