namespace Identity.Application.SystemActions.Commands.UpdateSystemAction
{
    internal sealed class UpdateSystemActionCommandValidator : AbstractValidator<UpdateSystemActionCommand>
    {
        public UpdateSystemActionCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        }
    }
}
