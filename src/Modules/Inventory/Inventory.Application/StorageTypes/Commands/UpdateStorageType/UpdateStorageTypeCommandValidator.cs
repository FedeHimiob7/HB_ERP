using FluentValidation;

namespace Inventory.Application.StorageTypes.Commands.UpdateStorageType
{
    public class UpdateStorageTypeCommandValidator : AbstractValidator<UpdateStorageTypeCommand>
    {
        public UpdateStorageTypeCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(250).When(x => x.Description != null);
        }
    }
}
