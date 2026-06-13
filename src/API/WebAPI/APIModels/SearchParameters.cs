namespace WebAPI.APIModels
{
    public record SearchParameters(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool IsDescending = false
    );
}
