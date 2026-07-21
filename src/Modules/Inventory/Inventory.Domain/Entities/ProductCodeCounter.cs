using MasterData.Domain.VO;

namespace Inventory.Domain.Entities
{
    public sealed class ProductCodeCounter
    {
        private ProductCodeCounter() { }

        private ProductCodeCounter(
            ProductServiceLineId pslId,
            int pslSequenceNumber,
            DateOnly date,
            int itemNumberByDay,
            string generatedCode)
        {
            PslId = pslId;
            PslSequenceNumber = pslSequenceNumber;
            Date = date;
            ItemNumberByDay = itemNumberByDay;
            GeneratedCode = generatedCode;
            IsConsumed = false;
            CreatedAt = DateTime.UtcNow;
        }

        public ProductServiceLineId PslId { get; private set; }
        public int PslSequenceNumber { get; private set; }
        public DateOnly Date { get; private set; }
        public int ItemNumberByDay { get; private set; }
        public string GeneratedCode { get; private set; } = string.Empty;
        public bool IsConsumed { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public static ProductCodeCounter Create(
            ProductServiceLineId pslId,
            int pslSequenceNumber,
            DateOnly date,
            int itemNumberByDay,
            string generatedCode)
            => new(pslId, pslSequenceNumber, date, itemNumberByDay, generatedCode);

        public void Consume() => IsConsumed = true;
    }
}
