namespace MasterData.Application.Cities.Models
{
    public record PagedCitiesResult(
        IReadOnlyList<CityResponse> Items,
        int TotalCount);
}
