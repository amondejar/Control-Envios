using ControlEnvios.Domain.Entities;
using ControlEnvios.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.IntegrationTests.Persistence;

/// <summary>
/// Validación del modelo EF Core sin conexión a BD: fuerza <c>OnModelCreating</c> con el proveedor
/// SqlServer para comprobar que las configuraciones Fluent (mapeo de tablas/columnas) son consistentes.
/// </summary>
public class BasculaDbContextModelTests
{
    private static BasculaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BasculaDbContext>()
            // Cadena ficticia: no se abre conexión, solo se construye el modelo.
            .UseSqlServer("Server=.;Database=Bascula;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;
        return new BasculaDbContext(options);
    }

    [Fact]
    public void El_modelo_se_construye_sin_errores()
    {
        using var context = CreateContext();

        var entityTypes = context.Model.GetEntityTypes().ToList();

        // 9 entidades de dominio con clave (más el tipo sin clave del SP de consulta).
        Assert.Equal(9, entityTypes.Count(e => e.FindPrimaryKey() is not null));
    }

    [Theory]
    [InlineData(typeof(Proveedor), "PROVEEDOR")]
    [InlineData(typeof(Envio), "ENVIOMERCANCIA")]
    [InlineData(typeof(Articulo), "ARTICULO")]
    [InlineData(typeof(CupoProveedor), "CUPOPROVEEDOR")]
    [InlineData(typeof(CupoGlobal), "CUPOKG")]
    [InlineData(typeof(Usuario), "USUARIOS")]
    [InlineData(typeof(Modulo), "MODULO")]
    [InlineData(typeof(EstadoMercancia), "ESTADOMERCANCIA")]
    [InlineData(typeof(PesadaBascula), "PESADABASCULA")]
    public void Cada_entidad_mapea_a_su_tabla_legacy(Type clrType, string tabla)
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(clrType);

        Assert.NotNull(entityType);
        Assert.Equal(tabla, entityType!.GetTableName());
        Assert.NotEmpty(entityType.FindPrimaryKey()!.Properties);
    }

    [Fact]
    public void Columnas_con_nombres_legacy_se_respetan()
    {
        using var context = CreateContext();
        var envio = context.Model.FindEntityType(typeof(Envio))!;

        // El nombre de propiedad limpio se mapea a la columna legacy (incluido el enum a int).
        Assert.Equal("IDENVIO", envio.FindProperty(nameof(Envio.Id))!.GetColumnName());
        Assert.Equal("KILOSENVIADOS", envio.FindProperty(nameof(Envio.KilosEnviados))!.GetColumnName());
        Assert.Equal("ESTADO", envio.FindProperty(nameof(Envio.Estado))!.GetColumnName());

        // Typo intencionado del esquema legacy preservado.
        var cupo = context.Model.FindEntityType(typeof(CupoProveedor))!;
        Assert.Equal("FEHAFINALCUPO", cupo.FindProperty(nameof(CupoProveedor.FechaFin))!.GetColumnName());
    }
}
