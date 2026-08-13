using MasterData.Application.Countries.Commands.CreateCountry;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateCountryCommandHandlerTests
    {
        private readonly ICountryRepository _repository = Substitute.For<ICountryRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();

        private CreateCountryCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenNameIsEmpty_ReturnsNameIsRequired()
        {
            var result = await CreateHandler().Handle(new CreateCountryCommand(""), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CountryErrors.NameIsRequired.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_PersistsCountryAndReturnsId()
        {
            var result = await CreateHandler().Handle(new CreateCountryCommand("Venezuela"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);

            await _repository.Received(1).AddAsync(Arg.Any<MasterData.Domain.Entities.Country>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
