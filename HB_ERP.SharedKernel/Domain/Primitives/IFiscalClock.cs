namespace HB_ERP.SharedKernel.Domain.Primitives
{
    // Fechas de negocio/fiscales (tasa de cambio, documentos fiscales) deben expresarse en hora Venezuela,
    // no en la hora local del servidor ni en UTC crudo — de lo contrario los cortes "por día" (Libro de Ventas,
    // conciliación con Reporte Z, vigencia de tasa de cambio) quedan desalineados con el calendario venezolano.
    // Las marcas de auditoría técnica (CreatedAt/ModifiedAt) siguen en UTC vía DateTime.UtcNow — eso no cambia.
    public interface IFiscalClock
    {
        DateTime UtcNow { get; }
        DateTime VenezuelaNow { get; }
        DateOnly VenezuelaToday { get; }
    }
}
