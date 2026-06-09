using Ardalis.GuardClauses;
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
    public sealed class Currency : AggregateRoot<CurrencyId>
    {
        private Currency() { }

        private Currency(CurrencyId id, string code, string name, string symbol, bool isActive)
            : base(id)
        {
            Code = code;
            Name = name;
            Symbol = symbol;
            IsActive = isActive;
        }

        public string Code { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string Symbol { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public static ErrorOr<Currency> Create(string code, string name, string symbol)
        {
            if (string.IsNullOrWhiteSpace(name))
                return CurrencyErrors.NameIsRequired;

            if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
                return CurrencyErrors.InvalidCode;

            var currency = new Currency(
                CurrencyId.New(),
                code.ToUpper(),
                name,
                symbol,
                isActive: true);

            
            currency.Raise(new CurrencyCreatedDomainEvent(
                currency.Id,
                currency.Code,
                currency.Name));

            return currency;
        }

        public static Currency CreateExisting(Guid id, string code, string name, string symbol, bool isActive)
        {
            return new Currency(
                CurrencyId.Create(id), 
                code,
                name,
                symbol,
                isActive);
        }

        public ErrorOr<Success> UpdateDetails(string name, string symbol)
        {
            if (string.IsNullOrWhiteSpace(name))
                return CurrencyErrors.NameIsRequired;

            Name = name;
            Symbol = symbol;

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
