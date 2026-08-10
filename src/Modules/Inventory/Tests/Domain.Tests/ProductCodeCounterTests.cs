using Inventory.Domain.Entities;
using MasterData.Domain.VO;
using Xunit;

namespace Domain.Tests
{
    public sealed class ProductCodeCounterTests
    {
        private static readonly ProductServiceLineId SamplePslId = ProductServiceLineId.New();
        private static readonly DateOnly SampleDate = new(2026, 8, 5);

        [Fact]
        public void Create_SetsAllFieldsAndStartsUnconsumed()
        {
            var counter = ProductCodeCounter.Create(
                SamplePslId,
                pslSequenceNumber: 5,
                SampleDate,
                itemNumberByDay: 3,
                generatedCode: "2026805-5-7-3");

            Assert.Equal(SamplePslId, counter.PslId);
            Assert.Equal(5, counter.PslSequenceNumber);
            Assert.Equal(SampleDate, counter.Date);
            Assert.Equal(3, counter.ItemNumberByDay);
            Assert.Equal("2026805-5-7-3", counter.GeneratedCode);
            Assert.False(counter.IsConsumed);
        }

        [Fact]
        public void Consume_SetsIsConsumedTrue()
        {
            var counter = ProductCodeCounter.Create(
                SamplePslId,
                pslSequenceNumber: 1,
                SampleDate,
                itemNumberByDay: 1,
                generatedCode: "2026805-1-1-1");

            counter.Consume();

            Assert.True(counter.IsConsumed);
        }
    }
}
