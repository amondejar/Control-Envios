using ControlEnvios.Domain.Entities;

namespace ControlEnvios.Application.Abstractions.Persistence;

public interface IEnvioRepository
{
    Task<Envio?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Envíos con <c>FECHAENVIO</c> a partir de la fecha indicada (hoy en adelante).</summary>
    Task<IReadOnlyList<Envio>> GetDesdeFechaAsync(DateOnly desde, CancellationToken ct = default);

    Task AddAsync(Envio envio, CancellationToken ct = default);
    void Update(Envio envio);
    void Remove(Envio envio);
}
