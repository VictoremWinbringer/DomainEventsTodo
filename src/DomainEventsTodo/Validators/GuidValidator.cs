using System;
using DomainEventsTodo.Domain;
using DomainEventsTodo.Repositories.Abstract;
using DomainEventsTodo.ViewModels;
using FluentValidation;

namespace DomainEventsTodo.Validators
{
    public class TodoSearchValidator : AbstractValidator<TodoSearchVm>
    {
        private readonly ITodoRepository _repository;

        public TodoSearchValidator(ITodoRepository repository)
        {
            _repository = repository;

            RuleFor(s => s.Id).NotEmpty()
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("IsNullOrEmpty"), nameof(TodoSearchVm.Id))); ;

            RuleFor(s => s.Id).Must(g => _repository[g] != null)
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("NotFound"), nameof(Todo))); ;
        }
    }
}