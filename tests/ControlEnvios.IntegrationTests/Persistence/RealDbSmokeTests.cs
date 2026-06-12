using ControlEnvios.Application.Autenticacion;
using ControlEnvios.Domain.Enums;
using ControlEnvios.Infrastructure.Persistence;
using ControlEnvios.Infrastructure.Persistence.Repositories;
using ControlEnvios.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.IntegrationTests.Persistence;

/// <summary>
/// Validación del mapeo contra una BD real. Se <b>omite</b> si no está la variable de entorno
/// <c>BASCULA_TEST_CONN</c> (no rompe la CI). Cada consulta materializa una fila, validando los
/// nombres de columna y los conversores de fecha/hora contra el esquema real de <c>Bascula</c>.
/// </summary>
public class RealDbSmokeTests
{
    private static string? Conn => Environment.GetEnvironmentVariable("BASCULA_TEST_CONN");

    private static BasculaDbContext Crear() =>
        new(new DbContextOptionsBuilder<BasculaDbContext>().UseSqlServer(Conn).Options);

    [Fact]
    public async Task Conecta_y_consulta_todas_las_tablas()
    {
        if (string.IsNullOrWhiteSpace(Conn))
        {
            return; // sin BD configurada → omitido
        }

        await using var ctx = Crear();
        Assert.True(await ctx.Database.CanConnectAsync());

        await ctx.Proveedores.Take(1).ToListAsync();
        await ctx.Usuarios.Take(1).ToListAsync();
        await ctx.Articulos.Take(1).ToListAsync();
        await ctx.Envios.Take(1).ToListAsync();
        await ctx.CuposProveedor.Take(1).ToListAsync();
        await ctx.CupoGlobal.Take(1).ToListAsync();
        await ctx.Modulos.Take(1).ToListAsync();
        await ctx.EstadosMercancia.Take(1).ToListAsync();
        await ctx.PesadasBascula.Take(1).ToListAsync();
    }

    [Fact]
    public async Task Login_real_de_un_proveedor_funciona()
    {
        if (string.IsNullOrWhiteSpace(Conn))
        {
            return; // sin BD configurada → omitido
        }

        await using var ctx = Crear();

        // Tomamos un proveedor real con contraseña (texto plano) y validamos el login completo,
        // sin exponer la credencial. Re-hash desactivado (AuthOptions por defecto).
        var prov = await ctx.Proveedores.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Password != null && p.Password != "");
        Assert.NotNull(prov);

        var auth = new AuthService(
            new ProveedorRepository(ctx),
            new UsuarioRepository(ctx),
            new Pbkdf2PasswordHasher(),
            new UnitOfWork(ctx),
            new AuthOptions());

        var resultado = await auth.AutenticarAsync(prov!.Codigo, prov.Password!);

        Assert.True(resultado.Exito);
        Assert.Equal(RolUsuario.Proveedor, resultado.Rol);
        Assert.Equal(prov.Codigo, resultado.Identificador);
    }
}
