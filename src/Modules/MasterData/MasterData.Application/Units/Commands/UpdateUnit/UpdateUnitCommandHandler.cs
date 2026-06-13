using ErrorOr;
using MasterData.Application.Interfaces;
using MasterData.Application.Units.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Commands.UpdateUnit
{
    internal sealed class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, ErrorOr<UnitResponse>>
    {
        private readonly IUnitRepository _repository;
        private readonly IMasterDataUnitOfWork _unitOfWork;

        public UpdateUnitCommandHandler(IUnitRepository repository, IMasterDataUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<UnitResponse>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
            var unit = await _repository.GetByIdAsync(UnitId.Create(request.Id), cancellationToken);
            if (unit is null) return UnitErrors.NotFound;

            var updateResult = unit.UpdateDetails(request.Name, request.Description);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(unit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UnitResponse(unit.Id.Value, unit.Name, unit.Description);
        }
    }
}
