using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.Infrastructure.Persistence.Repositories;

internal sealed class UsuarioRepository(BasculaDbContext context) : IUsuarioRepository
{
    public Task<Usuario?> GetByAliasAsync(string alias, CancellationToken ct = default) =>
        context.Usuarios.FirstOrDefaultAsync(u => u.Alias == alias, ct);
}
