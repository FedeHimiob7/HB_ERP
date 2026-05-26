
using HB_ERP.SharedKernel.Domain.Primitives;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics; 
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HB_ERP.SharedKernel.Infrastructure.Interceptors;

public sealed class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await PublishDomainEvents(eventData.Context, cancellationToken);
        }
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishDomainEvents(DbContext dbContext, CancellationToken cancellationToken)
    {
        // 1. Obtenemos todas las entidades rastreadas que implementen la interfaz de eventos
        var entries = dbContext.ChangeTracker.Entries()
            .Where(entry => entry.Entity is IHasDomainEvents hasEvents && hasEvents.DomainEvents.Any())
            .Select(entry => (IHasDomainEvents)entry.Entity)
            .ToList();

        // 🛑 COLOCA UN BREAKPOINT AQUÍ: Mira si la variable 'entries' ahora sí captura la entidad Currency
        if (!entries.Any())
        {
            return;
        }

        // 2. Extraemos todos los eventos
        var domainEvents = entries
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        // 3. Limpiamos los eventos de las entidades para evitar ejecuciones duplicadas
        entries.ForEach(entity => entity.ClearDomainEvents());

        var publisher = dbContext.GetService<IPublisher>();

        // 4. Publicamos en MediatR (Memoria local)
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}

