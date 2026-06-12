using ControlEnvios.Application.Abstractions.Persistence;

namespace ControlEnvios.Infrastructure.Persistence;

internal sealed class UnitOfWork(BasculaDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}
