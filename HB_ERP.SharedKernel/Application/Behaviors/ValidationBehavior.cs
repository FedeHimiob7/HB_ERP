using ErrorOr;
using FluentValidation;
using HB_ERP.SharedKernel.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HB_ERP.SharedKernel.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
                : IPipelineBehavior<TRequest, TResponse>
                where TRequest : IRequest<TResponse>
                where TResponse : IErrorOr
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = results
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (!failures.Any())
                return await next();

            var errors = failures
                .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
                .ToList();

            
            _logger.LogWithData(
                LogLevel.Warning,
                $"Validación rechazada en {typeof(TRequest).Name}",
                new { Request = request, Errors = errors } 
            );

            return (dynamic)ErrorOrFactory(errors);
        }

        private static object ErrorOrFactory(List<Error> errors)
            => typeof(TResponse)
                .GetMethod("From", new[] { typeof(List<Error>) })
                !.Invoke(null, new object[] { errors })!;
    }
}
