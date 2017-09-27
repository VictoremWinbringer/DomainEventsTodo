using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainEventsTodo.Domain;
using DomainEventsTodo.Repositories.Abstract;
using DomainEventsTodo.ViewModels;
using FluentValidation;

namespace DomainEventsTodo.Validators
{
    public class TodoValidator : AbstractValidator<TodoPostPutVm>
    {
        private const int MIN_DESCRIPTION_LENGTH = 3;
        private const int MAX_DESCRIPTION_LENGTH = 255;
        private readonly ITodoRepository _repository;
        public TodoValidator(ITodoRepository repository)
        {
            _repository = repository;

            RuleFor(t => t).NotNull()
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("IsNullOrEmpty"), "Todo"));

            RuleFor(t => t.Description).NotEmpty()
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("IsNullOrEmpty"), nameof(TodoPostPutVm.Description)));

            RuleFor(t => t.Description).MinimumLength(MIN_DESCRIPTION_LENGTH)
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("MinLength"),
                    nameof(TodoPostPutVm.Description), MIN_DESCRIPTION_LENGTH));

            RuleFor(t => t.Description).MaximumLength(MAX_DESCRIPTION_LENGTH)
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("MaxLength"), nameof(TodoPostPutVm.Description), MAX_DESCRIPTION_LENGTH));

            RuleFor(t => t.Description).Must(t => _repository.All().All(todo => todo.Memento.Description != t))
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("NotUnique"), nameof(TodoPostPutVm.Description)));
        }
    }
}
