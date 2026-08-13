using MasterData.Application.Branches.Queries.GetAll;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetAllBranchesQueryHandlerTests
    {
        private readonly IBranchRepository _repository = Substitute.For<IBranchRepository>();

        private GetAllBranchesQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_MapsAllBranchesToResponse()
        {
            var companyGuid = Guid.NewGuid();
            var branches = new List<Branch>
            {
                Branch.CreateExisting(Guid.NewGuid(), companyGuid, "Sucursal 1", "Direccion 1", 1, isActive: true),
                Branch.CreateExisting(Guid.NewGuid(), companyGuid, "Sucursal 2", "Direccion 2", 2, isActive: true),
            };
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(branches);

            var result = await CreateHandler().Handle(new GetAllBranchesQuery(), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(2, result.Value.Count);
            Assert.Equal("Sucursal 1", result.Value[0].Name);
            Assert.Equal("Sucursal 2", result.Value[1].Name);
        }
    }
}
