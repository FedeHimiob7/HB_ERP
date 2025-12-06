

namespace Identity.Application.Common
{
    public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
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

            return (dynamic)ErrorOrFactory(errors);
        }

        private static object ErrorOrFactory(List<Error> errors)
            => typeof(TResponse)
                .GetMethod("From", new[] { typeof(List<Error>) })
                !.Invoke(null, new object[] { errors })!;
    }
}
