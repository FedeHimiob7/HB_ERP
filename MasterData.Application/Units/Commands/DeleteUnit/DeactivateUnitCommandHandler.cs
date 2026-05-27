using ErrorOr;
using HB_ERP.SharedKernel.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Commands.DeleteUnit
{
    internal sealed class DeactivateUnitCommandHandler : IRequestHandler<DeactivateUnitCommand, ErrorOr<Success>>
    {
        private readonly IUnitRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateUnitCommandHandler(IUnitRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateUnitCommand request, CancellationToken cancellationToken)
        {
            var unit = await _repository.GetByIdAsync(UnitId.Create(request.Id), cancellationToken);
            if (unit is null) return UnitErrors.NotFound;

            unit.Deactivate();

            await _repository.UpdateAsync(unit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
