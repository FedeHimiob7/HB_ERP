namespace WebAPI.APIModels
{
    public record SearchParameters(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool IsDescending = false
    );
}
