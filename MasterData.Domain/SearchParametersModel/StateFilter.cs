using HB_ERP.SharedKernel.Domain.Common;
using MasterData.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.SearchParametersModel
{
    public record StateFilter : PaginationFilter
    {
        public CountryId? CountryId { get; init; }
        public bool? IsActive { get; init; }

        public StateFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            CountryId? countryId = null,
            bool? isActive = null)
            : base(pageNumber, pageSize, searchTerm)
        {
            CountryId = countryId;
            IsActive = isActive;
        }
    }
}
