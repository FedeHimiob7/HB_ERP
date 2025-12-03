using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain
{
    public interface IUserEmailUniquenessChecker
    {
        Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default);
    }
}
