using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Events;
using MasterData.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.Entities
{
    public sealed class Country : AggregateRoot<CountryId>
    {
        private Country() { }

        private Country(CountryId id, string name, bool isActive)
            : base(id)
        {
            Name = name;
            IsActive = isActive;
        }

        public string Name { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public static ErrorOr<Country> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return CountryErrors.NameIsRequired;

            var country = new Country(
                CountryId.New(),
                name,
                isActive: true);

            country.Raise(new CountryCreatedDomainEvent(
                country.Id,
                country.Name));

            return country;
        }

        public static Country CreateExisting(Guid id, string name, bool isActive)
        {
            return new Country(
                CountryId.Create(id),
                name,
                isActive);
        }

        public ErrorOr<Success> UpdateDetails(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return CountryErrors.NameIsRequired;

            Name = name;

            return Result.Success;
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}
