using HB_ERP.SharedKernel.Domain.Common;
using Identity.Domain.Common;

namespace Identity.Application.Users.Commands.ResetUserPassword
{
    public sealed class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
    {
        public ResetUserPasswordCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=\[{\]};:<>|./?]).+$")
                .WithMessage(FeaturedMessage.PasswordInvalido);
        }
    }
}
