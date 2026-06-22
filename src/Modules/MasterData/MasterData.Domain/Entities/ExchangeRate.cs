using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Enums;
using MasterData.Domain.Events;
using MasterData.Domain.VO;

namespace MasterData.Domain.Entities
{
    public sealed class ExchangeRate : AggregateRoot<ExchangeRateId>
    {
        private ExchangeRate() { }

        private ExchangeRate(ExchangeRateId id, DateTime date, decimal rate, ExchangeRateSource source)
            : base(id)
        {
            RegisterDate = date;
            Rate = rate;
            Source = source;
        }

        public DateTime RegisterDate { get; private set; }
        public decimal Rate { get; private set; }
        public ExchangeRateSource Source { get; private set; }

        public static ErrorOr<ExchangeRate> Create(decimal rate, ExchangeRateSource source)
        {
            if (rate <= 0)
                return ExchangeRateErrors.RateMustBePositive;

            var exchangeRate = new ExchangeRate(ExchangeRateId.New(), DateTime.UtcNow, rate, source);
            exchangeRate.Raise(new ExchangeRateRegisteredDomainEvent(exchangeRate.Id, exchangeRate.RegisterDate, rate, source));


            return exchangeRate;
        }

        public static ExchangeRate CreateExisting(Guid id, DateTime registeredAt, decimal rate, ExchangeRateSource source)
            => new ExchangeRate(ExchangeRateId.Create(id), registeredAt, rate, source);
    }
}
