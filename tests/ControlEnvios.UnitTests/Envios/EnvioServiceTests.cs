using ControlEnvios.Application.Cupos;
using ControlEnvios.Application.Envios;
using ControlEnvios.Domain.Entities;
using ControlEnvios.Domain.Enums;
using ControlEnvios.UnitTests.Fakes;

namespace ControlEnvios.UnitTests.Envios;

public class EnvioServiceTests
{
    private static readonly DateOnly Fecha = new(2026, 6, 11);

    private static CrearEnvioRequest Request(decimal kilos) =>
        new("P001", "ART1", Fecha, new TimeOnly(10, 30), kilos, "1234ABC", null);

    private static EnvioService Crear(
        FakeEnvioRepository envios, FakeCupoRepository cupos, FakeUnitOfWork uow) =>
        new(envios, new CupoService(cupos), uow, TimeProvider.System);

    [Fact]
    public async Task Crear_con_cupo_excedido_no_persiste_y_devuelve_error()
    {
        var envios = new FakeEnvioRepository();
        var uow = new FakeUnitOfWork();
        var cupos = new FakeCupoRepository
        {
            CupoActivo = new CupoProveedor { Id = 1, CodigoProveedor = "P001", Activo = true, PorcentajeAsignado = 10 },
            CupoGlobalKilos = 100_000,  // asignado = 10.000
            KilosSemana = 9_000,
        };
        var sut = Crear(envios, cupos, uow);

        var r = await sut.CrearAsync(Request(2_000)); // 11.000 > 10.000

        Assert.False(r.Exito);
        Assert.Empty(envios.Envios);
        Assert.Equal(0, uow.Guardados);
    }

    [Fact]
    public async Task Crear_permitido_persiste_con_estado_enviado()
    {
        var envios = new FakeEnvioRepository();
        var uow = new FakeUnitOfWork();
        var cupos = new FakeCupoRepository { CupoActivo = null }; // sin cupo → permitido
        var sut = Crear(envios, cupos, uow);

        var r = await sut.CrearAsync(Request(5_000));

        Assert.True(r.Exito);
        var envio = Assert.Single(envios.Envios);
        Assert.Equal(EstadoEnvio.Enviado, envio.Estado);
        Assert.Equal("P001", envio.CodigoProveedor);
        Assert.Equal(1, uow.Guardados);
    }

    [Fact]
    public async Task Editar_envio_inexistente_devuelve_error()
    {
        var sut = Crear(new FakeEnvioRepository(), new FakeCupoRepository(), new FakeUnitOfWork());

        var r = await sut.EditarAsync(new EditarEnvioRequest(99, "P001", "ART1", Fecha, null, 1, null, null));

        Assert.False(r.Exito);
    }

    [Fact]
    public async Task Eliminar_envio_existente_lo_quita()
    {
        var envios = new FakeEnvioRepository();
        envios.Envios.Add(new Envio { Id = 5, CodigoProveedor = "P001", CodigoArticulo = "ART1", FechaEnvio = Fecha });
        var uow = new FakeUnitOfWork();
        var sut = Crear(envios, new FakeCupoRepository(), uow);

        var r = await sut.EliminarAsync(5);

        Assert.True(r.Exito);
        Assert.Empty(envios.Envios);
        Assert.Equal(1, uow.Guardados);
    }
}
