using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Events;
using MasterData.Domain.VO;

namespace MasterData.Domain.Entities
{
    public sealed class City : AggregateRoot<CityId>
    {
        private City() { }

        private City(CityId id, StateId stateId, string name, bool isActive)
            : base(id)
        {
            StateId = stateId;
            Name = name;
            IsActive = isActive;
        }

        public StateId StateId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public static ErrorOr<City> Create(StateId stateId, string name)
        {
            if (stateId.Value == Guid.Empty)
                return CityErrors.InvalidState;

            if (string.IsNullOrWhiteSpace(name))
                return CityErrors.NameIsRequired;

            var city = new City(
                CityId.New(),
                stateId,
                name,
                isActive: true);

            city.Raise(new CityCreatedDomainEvent(city.Id, city.StateId, city.Name));

            return city;
        }

        public static City CreateExisting(Guid id, Guid stateId, string name, bool isActive)
        {
            return new City(
                CityId.Create(id),
                StateId.Create(stateId),
                name,
                isActive);
        }

        public ErrorOr<Success> UpdateDetails(StateId stateId, string name)
        {
            if (stateId.Value == Guid.Empty)
                return CityErrors.InvalidState;

            if (string.IsNullOrWhiteSpace(name))
                return CityErrors.NameIsRequired;

            StateId = stateId;
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
