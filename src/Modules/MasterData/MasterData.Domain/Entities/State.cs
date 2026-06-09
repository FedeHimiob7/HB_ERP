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
    public sealed class State : AggregateRoot<StateId>
    {
        private State() { }

        private State(StateId id, CountryId countryId, string code, string name, bool isActive)
            : base(id)
        {
            CountryId = countryId;
            Code = code;
            Name = name;
            IsActive = isActive;
        }

        public CountryId CountryId { get; private set; }
        public string Code { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public static ErrorOr<State> Create(CountryId countryId, string code, string name)
        {
            if (countryId.Value == Guid.Empty)
                return StateErrors.InvalidCountry;

            if (string.IsNullOrWhiteSpace(name))
                return StateErrors.NameIsRequired;

            if (string.IsNullOrWhiteSpace(code))
                return StateErrors.InvalidCode;

            var state = new State(
                StateId.New(),
                countryId,
                code.ToUpper(),
                name,
                isActive: true);

            state.Raise(new StateCreatedDomainEvent(
                state.Id,
                state.CountryId,
                state.Code,
                state.Name));

            return state;
        }

        public static State CreateExisting(Guid id, Guid countryId, string code, string name, bool isActive)
        {
            return new State(
                StateId.Create(id),
                CountryId.Create(countryId),
                code,
                name,
                isActive);
        }

        public ErrorOr<Success> UpdateDetails(CountryId countryId, string code, string name)
        {
            if (countryId.Value == Guid.Empty)
                return StateErrors.InvalidCountry;

            if (string.IsNullOrWhiteSpace(name))
                return StateErrors.NameIsRequired;

            if (string.IsNullOrWhiteSpace(code))
                return StateErrors.InvalidCode;

            CountryId = countryId;
            Code = code.ToUpper();
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
