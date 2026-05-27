using ErrorOr;
using HB_ERP.SharedKernel.Application.Interfaces;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Commands.CreateUnit
{
    internal sealed class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, ErrorOr<Guid>>
    {
        private readonly IUnitRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUnitCommandHandler(IUnitRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
            //le agregue el Domain.Entities por que hay un conflicto de ambiguedad con el Unit del MediatR
            var createResult = Domain.Entities.Unit.Create(request.Name, request.Description);
            if (createResult.IsError) return createResult.Errors;

            await _repository.AddAsync(createResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return createResult.Value.Id.Value;
        }
    }
}
