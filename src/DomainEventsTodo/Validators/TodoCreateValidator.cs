using DomainEventsTodo.Repositories.Abstract;
using DomainEventsTodo.ViewModels;
using FluentValidation;
using System.Linq;

namespace DomainEventsTodo.Validators
{
    public class TodoCreateValidator : AbstractValidator<TodoCreateVm>
    {
        private const int MIN_DESCRIPTION_LENGTH = 3;
        private const int MAX_DESCRIPTION_LENGTH = 255;
        private readonly ITodoRepository _repository;

        public TodoCreateValidator(ITodoRepository repository)
        {
            _repository = repository;

            RuleFor(t => t).NotNull()
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("IsNullOrEmpty"), "Todo"));

            RuleFor(t => t.Description).NotEmpty()
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("IsNullOrEmpty"), nameof(TodoCreateVm.Description)));

            RuleFor(t => t.Description).MinimumLength(MIN_DESCRIPTION_LENGTH)
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("MinLength"),
                    nameof(TodoCreateVm.Description), MIN_DESCRIPTION_LENGTH));

            RuleFor(t => t.Description).MaximumLength(MAX_DESCRIPTION_LENGTH)
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("MaxLength"), nameof(TodoCreateVm.Description), MAX_DESCRIPTION_LENGTH));

            RuleFor(t => t.Description).Must(t => _repository.All().All(todo => todo.Description != t))
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("NotUnique"), nameof(TodoCreateVm.Description)));
        }
    }
}
