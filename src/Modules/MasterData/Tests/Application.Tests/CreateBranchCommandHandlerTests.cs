using ErrorOr;
using MasterData.Application.Branches.Commands.CreateBranch;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CreateBranchCommandHandlerTests
    {
        private readonly IBranchRepository _branchRepository = Substitute.For<IBranchRepository>();
        private readonly ICompanyRepository _companyRepository = Substitute.For<ICompanyRepository>();

        private CreateBranchCommandHandler CreateHandler() => new(_branchRepository, _companyRepository);

        [Fact]
        public async Task Handle_WhenCompanyNotConfigured_ReturnsNotConfigured()
        {
            // D-1: Branch siempre cuelga de la única Company de la instalación — si todavía no
            // se configuró, no hay a quién asociarle la sucursal.
            _companyRepository.GetSingletonAsync(Arg.Any<CancellationToken>()).Returns((Company?)null);

            var result = await CreateHandler().Handle(new CreateBranchCommand("Sucursal Test", "Direccion Test"), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.NotConfigured.Code, result.FirstError.Code);
            await _branchRepository.DidNotReceive().ReserveNextSequenceNumberAndAddAsync(
                Arg.Any<Func<int, ErrorOr<Branch>>>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenValid_ReservesSequenceNumberAndReturnsBranchId()
        {
            var company = Company.CreateExisting("J-401027631-4", "Empresa Test", "Direccion Test", TaxpayerType.Ordinario);
            _companyRepository.GetSingletonAsync(Arg.Any<CancellationToken>()).Returns(company);

            // ReserveNextSequenceNumberAndAddAsync no recibe el número ya armado: recibe la FACTORY
            // (el handler se la pasa) que arma la Branch una vez que el repo sabe el próximo número.
            // Acá simulamos que el repo ya "reservó" el número 1 y ejecutamos la factory nosotros mismos.
            _branchRepository.ReserveNextSequenceNumberAndAddAsync(
                    Arg.Any<Func<int, ErrorOr<Branch>>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    var factory = callInfo.Arg<Func<int, ErrorOr<Branch>>>();
                    return factory(1);
                });

            var result = await CreateHandler().Handle(new CreateBranchCommand("Sucursal Test", "Direccion Test"), CancellationToken.None);

            Assert.False(result.IsError);
            Assert.NotEqual(Guid.Empty, result.Value);
        }
    }
}
