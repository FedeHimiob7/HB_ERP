namespace WebAPI.APIModels.MasterData.City
{
    public record GetCitiesPagedRequest(
        int PageNumber = 1,
        int PageSize = 10,
        Guid? StateId = null,
        Guid? CountryId = null,
        string? SearchTerm = null);
}
