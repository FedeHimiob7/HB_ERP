using MasterData.Application.Branches.Commands.UpdateBranch;
using MasterData.Application.Interfaces;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class UpdateBranchCommandHandlerTests
    {
        private readonly IBranchRepository _repository = Substitute.For<IBranchRepository>();
        private readonly IMasterDataUnitOfWork _unitOfWork = Substitute.For<IMasterDataUnitOfWork>();
        private readonly Guid _branchGuid = Guid.NewGuid();

        private UpdateBranchCommandHandler CreateHandler() => new(_repository, _unitOfWork);

        [Fact]
        public async Task Handle_WhenBranchDoesNotExist_ReturnsNotFound()
        {
            _repository.GetByIdAsync(Arg.Any<BranchId>(), Arg.Any<CancellationToken>()).Returns((Branch?)null);

            var command = new UpdateBranchCommand(_branchGuid, "Nombre Nuevo", "Direccion Nueva");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(BranchErrors.NotFound.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_UpdatesAndReturnsResponse()
        {
            var branch = Branch.CreateExisting(_branchGuid, Guid.NewGuid(), "Nombre Viejo", "Direccion Vieja", 1, isActive: true);
            _repository.GetByIdAsync(Arg.Any<BranchId>(), Arg.Any<CancellationToken>()).Returns(branch);

            var command = new UpdateBranchCommand(_branchGuid, "Nombre Nuevo", "Direccion Nueva");
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Equal("Nombre Nuevo", result.Value.Name);
            Assert.Equal("Direccion Nueva", result.Value.Address);
            // SequenceNumber no lo toca UpdateDetails — debe mantenerse igual al original.
            Assert.Equal(1, result.Value.SequenceNumber);

            await _repository.Received(1).UpdateAsync(branch, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
