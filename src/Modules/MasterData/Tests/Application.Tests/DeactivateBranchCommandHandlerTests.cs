using MasterData.Application.Branches.Commands.DeleteBranch;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class DeactivateBranchCommandHandlerTests
    {
        private readonly IBranchRepository _repository = Substitute.For<IBranchRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _branchGuid = Guid.NewGuid();

        private DeactivateBranchCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenBranchDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<BranchId>(), Arg.Any<CancellationToken>()).Returns((Branch?)null);

            var result = await CreateHandler().Handle(new DeactivateBranchCommand(_branchGuid), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(BranchErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_DeactivatesAndPersists()
        {
            var branch = Branch.CreateExisting(_branchGuid, Guid.NewGuid(), "Sucursal Test", "Direccion Test", 1, isActive: true);
            _repository.GetByIdAsync(Arg.Any<BranchId>(), Arg.Any<CancellationToken>()).Returns(branch);

            var result = await CreateHandler().Handle(new DeactivateBranchCommand(_branchGuid), CancellationToken.None);

            Assert.False(result.IsError);
            // Deactivate() es idempotente/en memoria — confirmamos que el estado del agregado cambió.
            Assert.False(branch.IsActive);

            await _repository.Received(1).UpdateAsync(branch, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
