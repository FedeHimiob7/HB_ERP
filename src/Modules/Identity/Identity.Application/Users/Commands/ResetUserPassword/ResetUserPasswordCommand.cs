namespace Identity.Application.Users.Commands.ResetUserPassword
{
    public sealed record ResetUserPasswordCommand(
        Guid UserId,
        string NewPassword
    ) : IRequest<ErrorOr<Success>>;
}
