
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics; 
using HB_ERP.SharedKernel.Domain.Primitives;
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
        var entitiesWithDomainEvents = dbContext.ChangeTracker.Entries<IHasDomainEvents>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = entitiesWithDomainEvents
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        entitiesWithDomainEvents.ForEach(entity => entity.ClearDomainEvents());

        // SOLUCIÓN DEFINITIVA: Extraemos el Endpoint de MassTransit.
        // Al publicar por aquí, MassTransit guarda el evento en la tabla Outbox
        var publishEndpoint = dbContext.GetService<IPublishEndpoint>();

        foreach (var domainEvent in domainEvents)
        {
            // MassTransit requiere que el evento sea un objeto para serializarlo correctamente en el Outbox
            await publishEndpoint.Publish((object)domainEvent, cancellationToken);
        }
    }
}

