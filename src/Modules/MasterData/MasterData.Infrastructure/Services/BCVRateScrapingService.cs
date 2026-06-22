using HtmlAgilityPack;
using MasterData.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace MasterData.Infrastructure.Services
{
    internal sealed class BCVRateScrapingService : IBCVRateScrapingService
    {
        private const string BcvUrl = "https://www.bcv.org.ve/";
        private const string XPathSelector = "//div[@id='dolar']//div[@class='row recuadrotsmc']";
        private const string DescendantTag = "strong";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BCVRateScrapingService> _logger;

        public BCVRateScrapingService(IHttpClientFactory httpClientFactory, ILogger<BCVRateScrapingService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<decimal> GetRateAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("BCV");
                var response = await client.GetAsync(BcvUrl, cancellationToken);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync(cancellationToken);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var node = doc.DocumentNode
                    .SelectSingleNode(XPathSelector)
                    ?.Descendants(DescendantTag)
                    .FirstOrDefault();

                if (node is null)
                {
                    _logger.LogWarning("BCV: no se encontró el nodo con la tasa. El HTML del BCV puede haber cambiado.");
                    return 0;
                }

                var raw = node.InnerText.Trim()
                    .Replace(".", string.Empty)   // quita separador de miles
                    .Replace(",", ".");            // coma decimal → punto

                if (decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var value))
                    return Math.Round(value, 4);

                _logger.LogWarning("BCV: no se pudo parsear el valor '{Raw}'.", raw);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BCV: error al obtener la tasa de cambio.");
                return 0;
            }
        }
    }
}
