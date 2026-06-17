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

        private Tax(TaxId id, string name, TaxType taxType, decimal rate, bool isActive) : base(id)
        {
            Name = name;
            TaxType = taxType;
            Rate = rate;
            IsActive = isActive;
        }

        public string Name { get; private set; } = string.Empty;
        public TaxType TaxType { get; private set; }
        public decimal Rate { get; private set; }
        public bool IsActive { get; private set; }

        public static ErrorOr<Tax> Create(string name, TaxType taxType, decimal rate)
        {
            if (string.IsNullOrWhiteSpace(name)) return TaxErrors.NameIsRequired;
            if (rate <= 0) return TaxErrors.RateMustBePositive;

            var tax = new Tax(TaxId.New(), name.Trim(), taxType, rate, isActive: true);
            tax.Raise(new TaxCreatedDomainEvent(tax.Id, tax.Name, tax.TaxType, tax.Rate));
            return tax;
        }

        public ErrorOr<Success> UpdateDetails(string name, TaxType taxType, decimal rate)
        {
            if (string.IsNullOrWhiteSpace(name)) return TaxErrors.NameIsRequired;
            if (rate <= 0) return TaxErrors.RateMustBePositive;

            Name = name.Trim();
            TaxType = taxType;
            Rate = rate;

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
