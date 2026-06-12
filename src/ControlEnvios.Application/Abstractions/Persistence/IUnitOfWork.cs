namespace ControlEnvios.Application.Abstractions.Persistence;

/// <summary>Confirma los cambios pendientes en la unidad de trabajo (transacción lógica).</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
