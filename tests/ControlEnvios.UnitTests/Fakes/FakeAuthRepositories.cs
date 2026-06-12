using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;

namespace ControlEnvios.UnitTests.Fakes;

internal sealed class FakeProveedorRepository : IProveedorRepository
{
    public Proveedor? Proveedor { get; set; }

    public Task<Proveedor?> GetByCodigoAsync(string codigo, CancellationToken ct = default) =>
        Task.FromResult(Proveedor is not null && Proveedor.Codigo == codigo ? Proveedor : null);

    public Task<IReadOnlyList<Proveedor>> GetSeleccionablesAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Proveedor>>([]);
}

internal sealed class FakeUsuarioRepository : IUsuarioRepository
{
    public Usuario? Usuario { get; set; }

    public Task<Usuario?> GetByAliasAsync(string alias, CancellationToken ct = default) =>
        Task.FromResult(Usuario is not null && Usuario.Alias == alias ? Usuario : null);
}
