using ControlEnvios.Application.Envios;
using ControlEnvios.Domain.Entities;
using ControlEnvios.Domain.Enums;
using ControlEnvios.UnitTests.Fakes;

namespace ControlEnvios.UnitTests.Envios;

public class CancelarEnvioServiceTests
{
    private static Envio EnvioComunicado() => new()
    {
        Id = 10,
        CodigoProveedor = "P001",
        CodigoArticulo = "ART1",
        FechaEnvio = new DateOnly(2026, 6, 1),
        Matricula = "1234ABC",
        Estado = EstadoEnvio.Comunicado,
    };

    [Fact]
    public async Task Cancela_envio_comunicado_y_notifica_al_proveedor()
    {
        var envios = new FakeEnvioRepository();
        envios.Envios.Add(EnvioComunicado());
        var notifs = new FakeNotificacionRepository();
        var uow = new FakeUnitOfWork();
        var sut = new CancelarEnvioService(envios, notifs, uow, TimeProvider.System);

        var r = await sut.CancelarAsync(10);

        Assert.True(r.Exito);
        Assert.Equal(EstadoEnvio.Cancelado, envios.Envios[0].Estado);
        var n = Assert.Single(notifs.Items);
        Assert.Equal("P001", n.CodigoProveedor);
        Assert.Equal(10, n.IdEnvio);
        Assert.False(n.Leida);
        Assert.Equal(1, uow.Guardados); // estado + notificación en una transacción
    }

    [Fact]
    public async Task Cancelar_envio_inexistente_devuelve_error()
    {
        var sut = new CancelarEnvioService(new FakeEnvioRepository(), new FakeNotificacionRepository(), new FakeUnitOfWork(), TimeProvider.System);

        var r = await sut.CancelarAsync(99);

        Assert.False(r.Exito);
    }

    [Fact]
    public async Task Cancelar_envio_ya_cancelado_no_duplica()
    {
        var envios = new FakeEnvioRepository();
        var envio = EnvioComunicado();
        envio.Estado = EstadoEnvio.Cancelado;
        envios.Envios.Add(envio);
        var notifs = new FakeNotificacionRepository();
        var uow = new FakeUnitOfWork();
        var sut = new CancelarEnvioService(envios, notifs, uow, TimeProvider.System);

        var r = await sut.CancelarAsync(10);

        Assert.False(r.Exito);
        Assert.Empty(notifs.Items);
        Assert.Equal(0, uow.Guardados);
    }
}
