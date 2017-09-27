using System;
using System.Linq;
using DomainEventsTodo.Dispatchers.Abstract;
using DomainEventsTodo.Domain;
using DomainEventsTodo.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace DomainEventsTodo.Dispatchers.Concrete
{
    public class DomainEventDispatcher : IDispatcher
    {
        private readonly IServiceProvider _container;

        public DomainEventDispatcher(IServiceProvider container)
        {
            _container = container;
        }

        public void Dispatch(BaseEvent domainEvent)
        {
            var handlerType = typeof(IHandler<>).MakeGenericType(domainEvent.GetType());
            var wrapperType = typeof(DomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _container.GetServices(handlerType);
            var wrappedHandlers = handlers
                .Cast<object>()
                .Select(handler => (DomainEventHandler)Activator.CreateInstance(wrapperType, handler));

            foreach (var handler in wrappedHandlers)
            {
                handler.Handle(domainEvent);
            }
        }

        private abstract class DomainEventHandler
        {
            public abstract void Handle(BaseEvent domainEvent);
        }

        private class DomainEventHandler<T> : DomainEventHandler
            where T : BaseEvent
        {
            private readonly IHandler<T> _handler;

            public DomainEventHandler(IHandler<T> handler)
            {
                _handler = handler;
            }

            public override void Handle(BaseEvent domainEvent)
            {
                _handler.Handle((T)domainEvent);
            }
        }
    }
}