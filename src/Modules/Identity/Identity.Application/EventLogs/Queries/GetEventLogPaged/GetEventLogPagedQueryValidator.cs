namespace Identity.Application.EventLogs.Queries.GetEventLogPaged
{
    internal class GetEventLogPagedQueryValidator : AbstractValidator<GetEventLogPagedQuery>
    {
        public GetEventLogPagedQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("El número de página debe ser mayor o igual a 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("El tamaño de página debe estar entre 1 y 100 registros.");
        }
    }
}
