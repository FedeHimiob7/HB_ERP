using HB_ERP.SharedKernel.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace HB_ERP.SharedKernel.Infrastructure.Extensions
{
    public static class EffectiveDatedQueryExtensions
    {
        public static Task<T?> GetEffectiveAsOfAsync<T>(
            this IQueryable<T> query, DateOnly asOfDate, CancellationToken cancellationToken = default)
            where T : class, IEffectiveDated
        {
            var upperBound = asOfDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

            return query
                .Where(e => e.EffectiveFrom < upperBound)
                .OrderByDescending(e => e.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken)!;
        }
    }
}
