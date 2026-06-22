using FluentValidation;

namespace Inventory.Application.Warehouses.Commands.UpdateWarehouse
{
    public class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
    {
        public UpdateWarehouseCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.ProductServiceLineId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(250).When(x => x.Description != null);
            RuleFor(x => x.Latitude).MaximumLength(50).When(x => x.Latitude != null);
            RuleFor(x => x.Longitude).MaximumLength(50).When(x => x.Longitude != null);
        }
    }
}
