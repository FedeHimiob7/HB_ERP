using HB_ERP.SharedKernel.Domain.Common;
using MasterData.Domain.VO;

namespace MasterData.Domain.SearchParametersModel
{
    public record CityFilter : PaginationFilter
    {
        public StateId? StateId { get; init; }
        public CountryId? CountryId { get; init; }

        public CityFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            StateId? stateId = null,
            CountryId? countryId = null)
            : base(pageNumber, pageSize, searchTerm)
        {
            StateId = stateId;
            CountryId = countryId;
        }
    }
}
