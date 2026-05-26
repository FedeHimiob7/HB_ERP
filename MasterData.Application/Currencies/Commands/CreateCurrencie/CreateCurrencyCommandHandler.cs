using ErrorOr;
using HB_ERP.SharedKernel.Application.Interfaces;
using HB_ERP.SharedKernel.IntegrationEvents;
using MassTransit;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Commands.CreateCurrencie
{
    internal sealed class CreateCurrencyCommandHandler
     : IRequestHandler<CreateCurrencyCommand, ErrorOr<Guid>>
    {
        private readonly ICurrencyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCurrencyCommandHandler> _logger;

        public CreateCurrencyCommandHandler(
            ICurrencyRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<CreateCurrencyCommandHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
        {
            var exists = await _repository.ExistsByCodeAsync(request.Code, cancellationToken);

            if (exists)
            {
                _logger.LogWarning("❌ Conflicto de negocio: Intento de crear una moneda duplicada ({CurrencyCode}).", request.Code);

                return Error.Conflict(
                    code: "Currency.Duplicate",
                    description: $"El código de moneda '{request.Code}' ya está registrado en el sistema.");
            }
            
            var currencyResult = Currency.Create(request.Code, request.Name, request.Symbol);

            if (currencyResult.IsError)
            {
                _logger.LogWarning("❌ Fallo en Dominio al crear la moneda: {@Errors}", currencyResult.Errors);
                return currencyResult.Errors;
            }

            await _repository.AddAsync(currencyResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("✅ Moneda {CurrencyCode} creada exitosamente con ID {CurrencyId}",
                request.Code,
                currencyResult.Value.Id.Value);

            return currencyResult.Value.Id.Value;
        }
    }
}
