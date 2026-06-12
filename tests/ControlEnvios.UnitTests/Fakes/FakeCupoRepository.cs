using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;

namespace ControlEnvios.UnitTests.Fakes;

/// <summary>Fake configurable de <see cref="ICupoRepository"/> (sin librería de mocking).</summary>
internal sealed class FakeCupoRepository : ICupoRepository
{
    public CupoProveedor? CupoActivo { get; set; }
    public decimal CupoGlobalKilos { get; set; }
    public decimal KilosSemana { get; set; }

    public Task<CupoProveedor?> GetCupoActivoProveedorAsync(string codigoProveedor, CancellationToken ct = default) =>
        Task.FromResult(CupoActivo);

    public Task<decimal> GetCupoGlobalKilosAsync(CancellationToken ct = default) =>
        Task.FromResult(CupoGlobalKilos);

    public Task<decimal> GetKilosEnviadosSemanaAsync(DateOnly inicio, DateOnly fin, string codigoProveedor, CancellationToken ct = default) =>
        Task.FromResult(KilosSemana);
}
