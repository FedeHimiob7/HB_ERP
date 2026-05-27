namespace WebAPI.APIModels.MasterData.State
{
    public record GetStatesPagedRequest(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? CountryId = null,
    string? SearchTerm = null
);
}
