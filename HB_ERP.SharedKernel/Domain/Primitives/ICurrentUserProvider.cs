using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HB_ERP.SharedKernel.Domain.Primitives
{
    public interface ICurrentUserProvider
    {
        string? UserId { get; }
        bool IsAuthenticated { get; }
    }
}
