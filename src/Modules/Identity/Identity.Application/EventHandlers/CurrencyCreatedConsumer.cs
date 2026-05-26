using HB_ERP.SharedKernel.Application.Interfaces;
using HB_ERP.SharedKernel.IntegrationEvents;
using Identity.Domain;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Application.EventHandlers
{
    public class CurrencyCreatedConsumer : IConsumer<CurrencyCreatedIntegrationEvent>
    {
        private readonly ILogger<CurrencyCreatedConsumer> _logger;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork; // <-- 1. Inyectamos el UnitOfWork

        public CurrencyCreatedConsumer(
            ILogger<CurrencyCreatedConsumer> logger,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<CurrencyCreatedIntegrationEvent> context)
        {
            var currencyId = context.Message.CurrencyId;
            var code = context.Message.Code;

            _logger.LogInformation("🚀 [IDENTITY] Mensaje recibido. Moneda {Code} creada.", code);

            var proofRole = Role.Create($"Moneda_Creada_{code}_{DateTime.UtcNow.Ticks}");

            await _roleRepository.AddAsync(proofRole);

            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("✅ Prueba física guardada en la BD de Identity.");
        }
    }
}
