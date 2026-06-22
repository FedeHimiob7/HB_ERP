using FluentValidation;

namespace Inventory.Application.StorageTypes.Commands.CreateStorageType
{
    public class CreateStorageTypeCommandValidator : AbstractValidator<CreateStorageTypeCommand>
    {
        public CreateStorageTypeCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(250).When(x => x.Description != null);
        }
    }
}
