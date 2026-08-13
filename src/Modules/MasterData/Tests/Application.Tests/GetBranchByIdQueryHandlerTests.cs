using MasterData.Application.Branches.Queries.GetById;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GetBranchByIdQueryHandlerTests
    {
        private readonly IBranchRepository _repository = Substitute.For<IBranchRepository>();
        private readonly Guid _branchGuid = Guid.NewGuid();

        private GetBranchByIdQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_WhenBranchDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<BranchId>(), Arg.Any<CancellationToken>()).Returns((Branch?)null);

            var result = await CreateHandler().Handle(new GetBranchByIdQuery(_branchGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(BranchErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenBranchExists_ReturnsResponse()
        {
            var branch = Branch.CreateExisting(_branchGuid, Guid.NewGuid(), "Sucursal Test", "Direccion Test", 3, isActive: true);
            _repository.GetByIdAsync(Arg.Any<BranchId>(), Arg.Any<CancellationToken>()).Returns(branch);

            var result = await CreateHandler().Handle(new GetBranchByIdQuery(_branchGuid), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal(_branchGuid, result.Value.Id);
            Assert.Equal(3, result.Value.SequenceNumber);
        }
    }
}
