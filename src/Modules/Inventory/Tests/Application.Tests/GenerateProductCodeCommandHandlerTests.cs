using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Products.Commands.GenerateProductCode;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class GenerateProductCodeCommandHandlerTests
    {
        // Substitute.For<T>() crea una versión "falsa" de la interfaz: no tiene lógica real,
        // devuelve valores por defecto (null, 0, etc.) hasta que le decimos qué devolver con .Returns(...).
        private readonly IProductCodeCounterRepository _counterRepository = Substitute.For<IProductCodeCounterRepository>();
        private readonly IProductServiceLineRepository _pslRepository = Substitute.For<IProductServiceLineRepository>();
        private readonly IBranchRepository _branchRepository = Substitute.For<IBranchRepository>();
        private readonly ICurrentUserProvider _currentUser = Substitute.For<ICurrentUserProvider>();

        private readonly Guid _pslGuid = Guid.NewGuid();
        private readonly Guid _branchGuid = Guid.NewGuid();

        // Arma el handler con los 4 mocks de arriba, para no repetirlo en cada test.
        private GenerateProductCodeCommandHandler CreateHandler()
            => new(_counterRepository, _pslRepository, _branchRepository, _currentUser);

        // El comando que le vamos a mandar al handler en cada test (mismo PSL y Branch siempre).
        private GenerateProductCodeCommand CreateCommand()
            => new(_pslGuid, _branchGuid);

        [Fact]
        public async Task Handle_WhenPslNotInCurrentUserPsls_ReturnsPslAccessDenied()
        {
            // El usuario actual solo tiene acceso a un PSL random, NO al _pslGuid que usamos en el comando.
            // Esto debería activar el primer guard del handler (chequeo de acceso) y cortar ahí.
            _currentUser.PslIds.Returns(new List<Guid> { Guid.NewGuid() });

            var result = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            Assert.True(result.IsError);
            // Comparamos por el código del error (no el objeto completo) para confirmar CUÁL error fue.
            Assert.Equal(CommonErrors.PslAccessDenied.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenPslDoesNotExist_ReturnsInvalidProductServiceLine()
        {
            // Ahora sí damos acceso al PSL, para pasar el primer guard y llegar al segundo.
            _currentUser.PslIds.Returns(new List<Guid> { _pslGuid });

            // Arg.Any<T>() = "no importa con qué lo llamen, siempre devolvé esto".
            // Simulamos que el PSL no existe en la base (devuelve null).
            _pslRepository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns((ProductServiceLine?)null);

            var result = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductErrors.InvalidProductServiceLine.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenBranchDoesNotExist_ReturnsInvalidBranch()
        {
            _currentUser.PslIds.Returns(new List<Guid> { _pslGuid });

            // CreateExisting reconstruye una entidad "como si ya existiera en la base", sin repetir
            // las validaciones de Create() — al handler solo le importa que no sea null.
            _pslRepository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns(ProductServiceLine.CreateExisting(_pslGuid, "desc", "PSL Test", isActive: true));

            // El PSL existe, pero la sucursal no — esto debería activar el tercer guard.
            _branchRepository.GetByIdAsync(Arg.Any<BranchId>(), Arg.Any<CancellationToken>())
                .Returns((Branch?)null);

            var result = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            Assert.True(result.IsError);
            Assert.Equal(ProductErrors.InvalidBranch.Code, result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_WhenValid_BuildsCodeWithPslAndBranchSequenceNumbers()
        {
            // Estos son los números "inventados" que vamos a hacer que el sistema use,
            // para después poder calcular a mano cuál debería ser el código final.
            const int pslSequenceNumber = 5;
            const int branchSequenceNumber = 7;
            const int dailyCounter = 3;

            // Esta vez las tres dependencias van a "responder bien" para llegar hasta el final.
            _currentUser.PslIds.Returns(new List<Guid> { _pslGuid });
            _pslRepository.GetByIdAsync(Arg.Any<ProductServiceLineId>(), Arg.Any<CancellationToken>())
                .Returns(ProductServiceLine.CreateExisting(_pslGuid, "desc", "PSL Test", isActive: true));

            var branch = Branch.CreateExisting(
                _branchGuid, Guid.NewGuid(), "Sucursal Test", "Direccion Test", branchSequenceNumber, isActive: true);
            _branchRepository.GetByIdAsync(Arg.Any<BranchId>(), Arg.Any<CancellationToken>())
                .Returns(branch);

            // Esta parte es la más particular: ReserveNextAsync no recibe un código ya armado,
            // recibe una FUNCIÓN (el handler se la pasa) que arma el código una vez que sabe los
            // números de secuencia. Como acá no hay repositorio real que la ejecute, la capturamos
            // nosotros mismos desde callInfo y la ejecutamos a mano con nuestros números de prueba.
            // Así probamos la lógica real de armado del código, no un string que inventamos aparte.
            _counterRepository.ReserveNextAsync(
                    Arg.Any<ProductServiceLineId>(),
                    Arg.Any<DateOnly>(),
                    Arg.Any<Func<int, int, string>>(),
                    Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    var codeFactory = callInfo.Arg<Func<int, int, string>>();
                    var code = codeFactory(pslSequenceNumber, dailyCounter);
                    return (pslSequenceNumber, dailyCounter, code);
                });

            var result = await CreateHandler().Handle(CreateCommand(), CancellationToken.None);

            // Armamos el código esperado por fuera, con los mismos números, para comparar.
            var today = DateOnly.FromDateTime(DateTime.Now);
            var expectedCode = $"{today.Year}{today.Month}{today.Day}-{pslSequenceNumber}-{branchSequenceNumber}-{dailyCounter}";

            Assert.False(result.IsError);
            Assert.Equal(expectedCode, result.Value.Code);

            // Received(1) no chequea un valor devuelto, chequea COMPORTAMIENTO:
            // "confirmá que se llamó exactamente una vez, con el PSL/Branch correctos" (no cualquiera).
            await _pslRepository.Received(1).GetByIdAsync(
                Arg.Is<ProductServiceLineId>(id => id == ProductServiceLineId.Create(_pslGuid)),
                Arg.Any<CancellationToken>());
            await _branchRepository.Received(1).GetByIdAsync(
                Arg.Is<BranchId>(id => id == BranchId.Create(_branchGuid)),
                Arg.Any<CancellationToken>());
        }
    }
}
