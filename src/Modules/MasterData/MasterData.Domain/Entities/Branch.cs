using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Events;
using MasterData.Domain.VO;

namespace MasterData.Domain.Entities
{
    public sealed class Branch : AggregateRoot<BranchId>
    {
        private Branch() { }

        private Branch(BranchId id, CompanyId companyId, string name, string address, int sequenceNumber, bool isActive)
            : base(id)
        {
            CompanyId = companyId;
            Name = name;
            Address = address;
            SequenceNumber = sequenceNumber;
            IsActive = isActive;
        }

        public CompanyId CompanyId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public int SequenceNumber { get; private set; }
        public bool IsActive { get; private set; }

        public static ErrorOr<Branch> Create(CompanyId companyId, string name, string address, int sequenceNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BranchErrors.NameIsRequired;

            if (string.IsNullOrWhiteSpace(address))
                return BranchErrors.AddressIsRequired;

            var branch = new Branch(
                BranchId.New(),
                companyId,
                name.Trim(),
                address.Trim(),
                sequenceNumber,
                isActive: true);

            branch.Raise(new BranchCreatedDomainEvent(
                branch.Id,
                branch.CompanyId,
                branch.Name,
                branch.SequenceNumber));

            return branch;
        }

        public static Branch CreateExisting(Guid id, Guid companyId, string name, string address, int sequenceNumber, bool isActive)
        {
            return new Branch(
                BranchId.Create(id),
                CompanyId.Create(companyId),
                name,
                address,
                sequenceNumber,
                isActive);
        }

        public ErrorOr<Success> UpdateDetails(string name, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BranchErrors.NameIsRequired;

            if (string.IsNullOrWhiteSpace(address))
                return BranchErrors.AddressIsRequired;

            Name = name.Trim();
            Address = address.Trim();

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
