using HB_ERP.SharedKernel.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Infrastructure.Persistence
{
    internal sealed class MasterDataEfUnitOfWork : IUnitOfWork
    {
        private readonly MasterDataDbContext _dbContext;

        public MasterDataEfUnitOfWork(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {           
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
