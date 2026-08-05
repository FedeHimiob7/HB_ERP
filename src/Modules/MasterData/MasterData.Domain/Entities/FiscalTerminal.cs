using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Enums;
using MasterData.Domain.Events;
using MasterData.Domain.VO;

namespace MasterData.Domain.Entities
{
    public sealed class FiscalTerminal : AggregateRoot<FiscalTerminalId>
    {
        private FiscalTerminal() { }

        private FiscalTerminal(FiscalTerminalId id, BranchId branchId, string name, EmissionMethod emissionMethod, bool isActive)
            : base(id)
        {
            BranchId = branchId;
            Name = name;
            EmissionMethod = emissionMethod;
            IsActive = isActive;
        }

        public BranchId BranchId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public EmissionMethod EmissionMethod { get; private set; }
        public bool IsActive { get; private set; }

        public static ErrorOr<FiscalTerminal> Create(BranchId branchId, string name, EmissionMethod emissionMethod)
        {
            if (branchId.Value == Guid.Empty)
                return FiscalTerminalErrors.InvalidBranch;

            if (string.IsNullOrWhiteSpace(name))
                return FiscalTerminalErrors.NameIsRequired;

            var fiscalTerminal = new FiscalTerminal(
                FiscalTerminalId.New(),
                branchId,
                name.Trim(),
                emissionMethod,
                isActive: true);

            fiscalTerminal.Raise(new FiscalTerminalCreatedDomainEvent(
                fiscalTerminal.Id,
                fiscalTerminal.BranchId,
                fiscalTerminal.Name,
                fiscalTerminal.EmissionMethod));

            return fiscalTerminal;
        }

        public static FiscalTerminal CreateExisting(Guid id, Guid branchId, string name, EmissionMethod emissionMethod, bool isActive)
        {
            return new FiscalTerminal(
                FiscalTerminalId.Create(id),
                BranchId.Create(branchId),
                name,
                emissionMethod,
                isActive);
        }

        public ErrorOr<Success> UpdateDetails(string name, EmissionMethod emissionMethod)
        {
            if (string.IsNullOrWhiteSpace(name))
                return FiscalTerminalErrors.NameIsRequired;

            Name = name.Trim();
            EmissionMethod = emissionMethod;

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
