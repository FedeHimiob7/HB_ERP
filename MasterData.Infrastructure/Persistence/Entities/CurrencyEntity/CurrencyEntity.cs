using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Infrastructure.Persistence.Entities.CurrencyEntity
{
    public class CurrencyEntity : IHasDomainEvents
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public required string Symbol { get; set; }
        public bool IsActive { get; set; }

        // ✅ Agregamos soporte para transportar los eventos hacia EF Core
        private readonly List<DomainEvent> _domainEvents = new();

        [NotMapped] // Evita que EF Core intente crear una columna en SQL para esto
        public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvents(IEnumerable<DomainEvent> events)
        {
            _domainEvents.AddRange(events);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
