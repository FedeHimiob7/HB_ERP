namespace WebAPI.APIModels.MasterData.ProductServiceLine
{
    public record GetProductServiceLinesPagedRequest(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null
    );
}
