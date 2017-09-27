using DomainEventsTodo.Domain.Events;
using DomainEventsTodo.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace DomainEventsTodo.Domain
{
    public class TodoCompleteService : IHandler<TodoComplete>
    {
        private readonly IHubContext<Notifier> _notifier;

        public TodoCompleteService(IHubContext<Notifier> notifier)
        {
            _notifier = notifier;
        }

        public void Handle(TodoComplete domainEvent)
        {
            _notifier.Clients.All.InvokeAsync("Notify", domainEvent.Memento.Description + " is complete").Wait();
        }
    }
}