namespace Identity.Application.Common.Interfaces
{
    public interface IPslExistenceChecker
    {
        Task<bool> ExistsAsync(Guid pslId, CancellationToken cancellationToken = default);
    }
}
