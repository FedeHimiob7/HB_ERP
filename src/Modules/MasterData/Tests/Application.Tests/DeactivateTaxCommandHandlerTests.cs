using MasterData.Application.Interfaces;
using MasterData.Application.Taxes.Commands.DeactivateTax;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateTaxCommandHandlerTests
    {
        private readonly ITaxRepository _repository = Substitute.For<ITaxRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _taxGuid = Guid.NewGuid();

        private DeactivateTaxCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenTaxDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<TaxId>(), Arg.Any<CancellationToken>()).Returns((Tax?)null);

            var result = await CreateHandler().Handle(new DeactivateTaxCommand(_taxGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(TaxErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var createResult = Tax.Create("IVA", TaxType.IVA);
            var tax = createResult.Value;
            _repository.GetByIdAsync(Arg.Any<TaxId>(), Arg.Any<CancellationToken>()).Returns(tax);

            var result = await CreateHandler().Handle(new DeactivateTaxCommand(_taxGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.False(tax.IsActive);

            await _repository.Received(1).UpdateAsync(tax, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
