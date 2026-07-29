using ErrorOr;
using MediatR;

namespace Inventory.Application.Products.Queries.CalculatePrices
{
    internal sealed class CalculatePricesQueryHandler : IRequestHandler<CalculatePricesQuery, ErrorOr<PriceCalculationResult>>
    {
        private const decimal ProfitPrice2 = 20m;
        private const decimal ProfitPrice3 = 10m;
        private const decimal ProfitPrice4 = 5m;

        public Task<ErrorOr<PriceCalculationResult>> Handle(CalculatePricesQuery request, CancellationToken cancellationToken)
        {
            decimal igtfRate = request.TaxList
                .Where(t => t.IsIGTF)
                .Select(t => t.Rate)
                .FirstOrDefault();

            decimal regularTaxSum = request.TaxList
                .Where(t => !t.IsIGTF)
                .Sum(t => t.Rate);

            bool hasTaxes = request.TaxList.Count > 0;

            decimal ApplyTax(decimal basePrice)
            {
                decimal amount = basePrice + (basePrice * regularTaxSum);
                if (igtfRate > 0)
                    amount += amount * igtfRate;
                return amount;
            }

            if (request.IsCost)
            {
                decimal cost = hasTaxes
                    ? Math.Round(ApplyTax(request.BaseAmount), 2)
                    : request.BaseAmount;

                return Task.FromResult<ErrorOr<PriceCalculationResult>>(
                    new PriceCalculationResult(cost, request.BaseAmount, 0, 0, 0, 0, 0, null));
            }

            decimal commission = request.Commission ?? 0;

            decimal CalcBase(decimal profitPercent) =>
                request.BaseAmount + (request.BaseAmount * (profitPercent / 100)) + commission;

            decimal price1Base = CalcBase(request.Profit ?? 0);
            decimal price2Base = CalcBase(ProfitPrice2);
            decimal price3Base = CalcBase(ProfitPrice3);
            decimal price4Base = CalcBase(ProfitPrice4);

            decimal Finalize(decimal b) =>
                hasTaxes ? Math.Round(ApplyTax(b), 2) : Math.Round(b, 2);

            var result = new PriceCalculationResult(
                Cost:   0,
                CostBase: null,
                Price1: Finalize(price1Base),
                Price2: Finalize(price2Base),
                Price3: Finalize(price3Base),
                Price4: Finalize(price4Base),
                Price5: request.BaseAmount,
                PriceBase: Math.Round(price1Base, 2));

            return Task.FromResult<ErrorOr<PriceCalculationResult>>(result);
        }
    }
}
