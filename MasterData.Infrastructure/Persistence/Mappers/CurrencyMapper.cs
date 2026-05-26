using MasterData.Domain.Entities;
using MasterData.Domain.VO;
using MasterData.Infrastructure.Persistence.Entities.CurrencyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Infrastructure.Persistence.Mappers
{
    internal static class CurrencyMapper
    {
        public static CurrencyEntity ToEntity(Currency currency)
        {
            var entity = new CurrencyEntity
            {
                Id = currency.Id.Value, // O como expongas el Guid
                Code = currency.Code,
                Name = currency.Name,
                Symbol = currency.Symbol,
                IsActive = currency.IsActive
            };

            // ✅ TRASPASO DE EVENTOS: Copiamos los eventos al Entity de DB
            if (currency.DomainEvents.Any())
            {
                entity.AddDomainEvents(currency.DomainEvents);
            }

            return entity;
        }

        public static Currency ToDomain(CurrencyEntity entity)
        {            
            return Currency.CreateExisting(
                entity.Id,
                entity.Code,
                entity.Name,
                entity.Symbol,
                entity.IsActive
            );
        }
    }
}
