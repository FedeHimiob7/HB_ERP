namespace WebAPI.APIModels.MasterData.Unit
{
    public record GetUnitsPagedRequest(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null);
}
