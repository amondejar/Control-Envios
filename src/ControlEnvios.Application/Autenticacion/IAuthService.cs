namespace ControlEnvios.Application.Autenticacion;

public interface IAuthService
{
    /// <summary>Autentica a un proveedor o usuario interno (gestor/producción) por usuario y contraseña.</summary>
    Task<ResultadoLogin> AutenticarAsync(string usuario, string password, CancellationToken ct = default);
}
