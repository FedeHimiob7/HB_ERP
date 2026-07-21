using MasterData.Domain.VO;

namespace Inventory.Domain.Repositories
{
    public interface IProductCodeCounterRepository
    {
        /// <summary>
        /// Atomically reserves the next counter value for a PSL.
        /// El codeFactory recibe (pslSequenceNumber, itemNumberByDay) y devuelve el string del código.
        /// Crea una fila con IsConsumed = false y el código generado.
        /// </summary>
        Task<(int PslSequenceNumber, int ItemNumberByDay, string GeneratedCode)> ReserveNextAsync(
            ProductServiceLineId pslId,
            DateOnly date,
            Func<int, int, string> codeFactory,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Marca la reserva como consumida. NO llama SaveChanges — el caller hace commit
        /// junto con el producto para atomicidad.
        /// Devuelve ItemNumberByDay si existe la reserva, null si el usuario editó el código.
        /// </summary>
        Task<int?> ConsumeAsync(
            string generatedCode,
            ProductServiceLineId pslId,
            CancellationToken cancellationToken = default);
    }
}
