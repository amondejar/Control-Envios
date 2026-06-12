using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;

namespace ControlEnvios.UnitTests.Fakes;

internal sealed class FakeEnvioRepository : IEnvioRepository
{
    public List<Envio> Envios { get; } = [];

    public Task<Envio?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(Envios.FirstOrDefault(e => e.Id == id));

    public Task<IReadOnlyList<Envio>> GetDesdeFechaAsync(DateOnly desde, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Envio>>(Envios.Where(e => e.FechaEnvio >= desde).ToList());

    public Task AddAsync(Envio envio, CancellationToken ct = default)
    {
        Envios.Add(envio);
        return Task.CompletedTask;
    }

    public void Update(Envio envio) { /* en memoria: misma referencia */ }

    public void Remove(Envio envio) => Envios.Remove(envio);
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int Guardados { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        Guardados++;
        return Task.FromResult(1);
    }
}
