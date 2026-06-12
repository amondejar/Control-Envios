using ControlEnvios.Application.Autenticacion;
using ControlEnvios.Domain.Entities;
using ControlEnvios.Domain.Enums;
using ControlEnvios.UnitTests.Fakes;

namespace ControlEnvios.UnitTests.Autenticacion;

public class AuthServiceTests
{
    private static AuthService Sut(FakeProveedorRepository prov, FakeUsuarioRepository usu, FakePasswordHasher? hasher = null) =>
        new(prov, usu, hasher ?? new FakePasswordHasher(), new FakeUnitOfWork());

    // LOGIN-01
    [Fact]
    public async Task Proveedor_valido_autentica_como_proveedor()
    {
        var prov = new FakeProveedorRepository { Proveedor = new Proveedor { Codigo = "P001", Nombre = "Cítricos SL", Password = "secret" } };
        var sut = Sut(prov, new FakeUsuarioRepository());

        var r = await sut.AutenticarAsync("P001", "secret");

        Assert.True(r.Exito);
        Assert.Equal(RolUsuario.Proveedor, r.Rol);
        Assert.Equal("P001", r.Identificador);
    }

    // LOGIN-02
    [Fact]
    public async Task Usuario_gestor_valido_autentica_como_gestor()
    {
        var usu = new FakeUsuarioRepository
        {
            Usuario = new Usuario { Alias = "gestor1", Password = "pw", AccesoWebAdmin = true, Perfil = PerfilUsuario.Gestor },
        };
        var sut = Sut(new FakeProveedorRepository(), usu);

        var r = await sut.AutenticarAsync("gestor1", "pw");

        Assert.True(r.Exito);
        Assert.Equal(RolUsuario.Gestor, r.Rol);
    }

    // LOGIN-03
    [Fact]
    public async Task Usuario_produccion_valido_autentica_como_produccion()
    {
        var usu = new FakeUsuarioRepository
        {
            Usuario = new Usuario { Alias = "prod1", Password = "pw", AccesoWebAdmin = true, Perfil = PerfilUsuario.Produccion },
        };
        var sut = Sut(new FakeProveedorRepository(), usu);

        var r = await sut.AutenticarAsync("prod1", "pw");

        Assert.True(r.Exito);
        Assert.Equal(RolUsuario.Produccion, r.Rol);
    }

    // LOGIN-04
    [Fact]
    public async Task Password_incorrecta_falla()
    {
        var prov = new FakeProveedorRepository { Proveedor = new Proveedor { Codigo = "P001", Password = "secret" } };
        var sut = Sut(prov, new FakeUsuarioRepository());

        var r = await sut.AutenticarAsync("P001", "mala");

        Assert.False(r.Exito);
        Assert.Equal(ResultadoLogin.MensajeCredencialesInvalidas, r.Motivo);
    }

    [Fact]
    public async Task Usuario_interno_sin_acceso_webadmin_falla()
    {
        var usu = new FakeUsuarioRepository
        {
            Usuario = new Usuario { Alias = "gestor1", Password = "pw", AccesoWebAdmin = false, Perfil = PerfilUsuario.Gestor },
        };
        var sut = Sut(new FakeProveedorRepository(), usu);

        var r = await sut.AutenticarAsync("gestor1", "pw");

        Assert.False(r.Exito);
    }

    [Theory]
    [InlineData("", "pw")]
    [InlineData("user", "")]
    public async Task Credenciales_vacias_fallan(string usuario, string password)
    {
        var sut = Sut(new FakeProveedorRepository(), new FakeUsuarioRepository());

        var r = await sut.AutenticarAsync(usuario, password);

        Assert.False(r.Exito);
    }

    [Fact]
    public async Task Login_con_password_heredada_rehashea_y_persiste()
    {
        var proveedor = new Proveedor { Codigo = "P001", Password = "secret" };
        var uow = new FakeUnitOfWork();
        var sut = new AuthService(
            new FakeProveedorRepository { Proveedor = proveedor },
            new FakeUsuarioRepository(),
            new FakePasswordHasher { Rehash = true },
            uow);

        var r = await sut.AutenticarAsync("P001", "secret");

        Assert.True(r.Exito);
        Assert.Equal("HASHED:secret", proveedor.Password); // re-hasheada
        Assert.Equal(1, uow.Guardados);                    // persistida
    }

    [Fact]
    public async Task Login_con_hash_actual_no_rehashea()
    {
        var proveedor = new Proveedor { Codigo = "P001", Password = "secret" };
        var uow = new FakeUnitOfWork();
        var sut = new AuthService(
            new FakeProveedorRepository { Proveedor = proveedor },
            new FakeUsuarioRepository(),
            new FakePasswordHasher { Rehash = false },
            uow);

        await sut.AutenticarAsync("P001", "secret");

        Assert.Equal("secret", proveedor.Password);
        Assert.Equal(0, uow.Guardados);
    }
}
