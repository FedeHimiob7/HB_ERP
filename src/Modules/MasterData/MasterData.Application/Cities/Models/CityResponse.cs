namespace MasterData.Application.Cities.Models
{
    public record CityResponse(
        Guid Id,
        Guid StateId,
        string Name);
}
