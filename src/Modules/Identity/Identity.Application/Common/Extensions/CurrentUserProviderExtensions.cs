using HB_ERP.SharedKernel.Domain.Primitives;

namespace Identity.Application.Common.Extensions
{
    public static class CurrentUserProviderExtensions
    {
        public static Guid? GetUserIdOrNull(this ICurrentUserProvider currentUserProvider)
            => Guid.TryParse(currentUserProvider.UserId, out var userId) ? userId : null;
    }
}
