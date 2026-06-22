namespace MasterData.Application.Interfaces
{
    public interface IBCVRateScrapingService
    {
        Task<decimal> GetRateAsync(CancellationToken cancellationToken = default);
    }
}
