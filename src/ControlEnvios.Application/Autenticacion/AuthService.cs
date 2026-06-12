using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Application.Abstractions.Security;
using ControlEnvios.Domain.Enums;

namespace ControlEnvios.Application.Autenticacion;

/// <summary>
/// Lógica de login extraída de <c>AccountController</c>. Verifica primero como proveedor y, si no,
/// como usuario interno (gestor o producción), exigiendo <c>AccesoWebAdmin</c>.
/// </summary>
/// <remarks>
/// A diferencia del legacy, no se emite ninguna credencial/cookie hasta confirmar las credenciales
/// (la emisión de la cookie es responsabilidad de la capa Web), y la contraseña se re-hashea de forma
/// progresiva al primer login válido si todavía estaba en texto plano (ver <see cref="IPasswordHasher"/>).
/// </remarks>
public sealed class AuthService(
    IProveedorRepository proveedorRepository,
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<ResultadoLogin> AutenticarAsync(string usuario, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
        {
            return ResultadoLogin.Fallido();
        }

        // 1) ¿Proveedor?
        var proveedor = await proveedorRepository.GetByCodigoAsync(usuario, ct);
        if (proveedor is not null && passwordHasher.Verificar(password, proveedor.Password))
        {
            await RehashearSiProcedeAsync(password, proveedor.Password, h => proveedor.Password = h, ct);
            return ResultadoLogin.Correcto(RolUsuario.Proveedor, proveedor.Codigo, proveedor.Nombre);
        }

        // 2) ¿Usuario interno (gestor / producción)?
        var interno = await usuarioRepository.GetByAliasAsync(usuario, ct);
        if (interno is not null
            && interno.AccesoWebAdmin
            && passwordHasher.Verificar(password, interno.Password)
            && interno.Perfil is PerfilUsuario.Gestor or PerfilUsuario.Produccion)
        {
            await RehashearSiProcedeAsync(password, interno.Password, h => interno.Password = h, ct);
            var rol = interno.Perfil == PerfilUsuario.Gestor ? RolUsuario.Gestor : RolUsuario.Produccion;
            return ResultadoLogin.Correcto(rol, interno.Alias, interno.Nombre);
        }

        return ResultadoLogin.Fallido();
    }

    private async Task RehashearSiProcedeAsync(string password, string? almacenada, Action<string> asignar, CancellationToken ct)
    {
        if (!passwordHasher.NecesitaRehash(almacenada))
        {
            return;
        }

        asignar(passwordHasher.Hashear(password));
        await unitOfWork.SaveChangesAsync(ct);
    }
}
