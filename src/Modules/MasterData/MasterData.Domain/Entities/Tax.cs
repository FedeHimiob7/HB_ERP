using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Enums;
using MasterData.Domain.Events;
using MasterData.Domain.VO;

namespace MasterData.Domain.Entities
{
    public sealed class Tax : AggregateRoot<TaxId>
    {
        private Tax() { }

        private Tax(TaxId id, string name, TaxType taxType, bool isActive) : base(id)
        {
            Name = name;
            TaxType = taxType;
            IsActive = isActive;
        }

        public string Name { get; private set; } = string.Empty;
        public TaxType TaxType { get; private set; }
        public bool IsActive { get; private set; }

        public static ErrorOr<Tax> Create(string name, TaxType taxType)
        {
            if (string.IsNullOrWhiteSpace(name)) return TaxErrors.NameIsRequired;

            var tax = new Tax(TaxId.New(), name.Trim(), taxType, isActive: true);
            tax.Raise(new TaxCreatedDomainEvent(tax.Id, tax.Name, tax.TaxType));
            return tax;
        }

        public ErrorOr<Success> UpdateDetails(string name, TaxType taxType)
        {
            if (string.IsNullOrWhiteSpace(name)) return TaxErrors.NameIsRequired;

            Name = name.Trim();
            TaxType = taxType;

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
