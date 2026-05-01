using Identity.Domain.Entities;
using Identity.Domain.VO;
using Identity.Infrastructure.Persistence.Entities.SystemActionEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Mappers
{
    internal static class SystemActionMapper
    {
        public static SystemActionEntity ToEntity(this SystemAction domain)
        {
            return new SystemActionEntity
            {
                Id = domain.Id.Value,
                Name = domain.Name.Value,
                Description = domain.Description,
                IsActive = domain.IsActive
            };
        }

        public static SystemAction ToDomain(this SystemActionEntity entity)
        {
            return SystemAction.CreateExisting(
                new ActionsId(entity.Id),
                ActionName.Create(entity.Name),
                entity.Description ?? string.Empty,
                entity.IsActive
            );
        }
    }
}
