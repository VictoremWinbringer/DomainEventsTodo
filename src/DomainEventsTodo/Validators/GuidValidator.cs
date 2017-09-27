using System;
using DomainEventsTodo.Repositories.Abstract;
using FluentValidation;

namespace DomainEventsTodo.Validators
{
    public class GuidValidator : AbstractValidator<Guid>
    {
        private readonly ITodoRepository _repository;

        public GuidValidator(ITodoRepository repository)
        {
            _repository = repository;

            RuleFor(g => g).NotEmpty()
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("IsNullOrEmpty"), nameof(Guid))); ;

            RuleFor(g => g).Must(g => _repository[g] != null)
                .WithMessage(string.Format(Properties.Resource.ResourceManager.GetString("NotFound"), "Todo")); ;
        }
    }
}