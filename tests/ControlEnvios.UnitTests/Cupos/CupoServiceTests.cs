using ControlEnvios.Application.Cupos;
using ControlEnvios.Domain.Entities;
using ControlEnvios.UnitTests.Fakes;

namespace ControlEnvios.UnitTests.Cupos;

public class CupoServiceTests
{
    private const string Proveedor = "P001";
    private static readonly DateOnly Fecha = new(2026, 6, 11);

    private static CupoProveedor CupoActivo(decimal porcentaje) => new()
    {
        Id = 1,
        CodigoProveedor = Proveedor,
        PorcentajeAsignado = porcentaje,
        Activo = true,
    };

    // ENV-01
    [Fact]
    public async Task Sin_cupo_activo_permite_el_envio()
    {
        var repo = new FakeCupoRepository { CupoActivo = null };
        var sut = new CupoService(repo);

        var r = await sut.ValidarEnvioAsync(Proveedor, kilosNuevoEnvio: 999_999, Fecha);

        Assert.True(r.Permitido);
    }

    // ENV-02: primer envío de la semana permitido aunque exceda (regla heredada).
    [Fact]
    public async Task Primer_envio_de_la_semana_se_permite_aunque_exceda()
    {
        var repo = new FakeCupoRepository
        {
            CupoActivo = CupoActivo(10),
            CupoGlobalKilos = 100_000,
            KilosSemana = 0, // aún no ha enviado esta semana
        };
        var sut = new CupoService(repo);

        var r = await sut.ValidarEnvioAsync(Proveedor, kilosNuevoEnvio: 999_999, Fecha);

        Assert.True(r.Permitido);
    }

    // ENV-03: suma dentro de la asignación → permitido.
    [Fact]
    public async Task Dentro_del_cupo_permite_el_envio()
    {
        var repo = new FakeCupoRepository
        {
            CupoActivo = CupoActivo(10),   // 10% de 100.000 = 10.000
            CupoGlobalKilos = 100_000,
            KilosSemana = 8_000,
        };
        var sut = new CupoService(repo);

        var r = await sut.ValidarEnvioAsync(Proveedor, kilosNuevoEnvio: 1_500, Fecha); // 9.500 <= 10.000

        Assert.True(r.Permitido);
        Assert.Equal(10_000m, r.KilosAsignados);
    }

    // ENV-04: suma supera la asignación (no primer envío) → excedido.
    [Fact]
    public async Task Supera_el_cupo_rechaza_el_envio()
    {
        var repo = new FakeCupoRepository
        {
            CupoActivo = CupoActivo(10),
            CupoGlobalKilos = 100_000,
            KilosSemana = 8_000,
        };
        var sut = new CupoService(repo);

        var r = await sut.ValidarEnvioAsync(Proveedor, kilosNuevoEnvio: 2_500, Fecha); // 10.500 > 10.000

        Assert.False(r.Permitido);
        Assert.Equal(ResultadoCupo.MensajeCupoExcedido, r.Motivo);
    }

    [Fact]
    public async Task El_limite_exacto_se_permite()
    {
        var repo = new FakeCupoRepository
        {
            CupoActivo = CupoActivo(10),
            CupoGlobalKilos = 100_000,
            KilosSemana = 8_000,
        };
        var sut = new CupoService(repo);

        var r = await sut.ValidarEnvioAsync(Proveedor, kilosNuevoEnvio: 2_000, Fecha); // 10.000 == 10.000

        Assert.True(r.Permitido);
    }

    // Regresión: cupo presente pero sin porcentaje no debe lanzar (NRE del legacy).
    [Fact]
    public async Task Cupo_activo_sin_porcentaje_permite_y_no_lanza()
    {
        var repo = new FakeCupoRepository
        {
            CupoActivo = new CupoProveedor { Id = 1, CodigoProveedor = Proveedor, Activo = true, PorcentajeAsignado = null },
            CupoGlobalKilos = 100_000,
            KilosSemana = 8_000,
        };
        var sut = new CupoService(repo);

        var r = await sut.ValidarEnvioAsync(Proveedor, kilosNuevoEnvio: 5_000, Fecha);

        Assert.True(r.Permitido);
    }
}
