using ControlEnvios.Infrastructure.Security;

namespace ControlEnvios.IntegrationTests.Security;

public class Pbkdf2PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _sut = new();

    [Fact]
    public void Hashear_y_verificar_correcto()
    {
        var hash = _sut.Hashear("Sup3rS3creta!");

        Assert.StartsWith("PBKDF2$", hash);
        Assert.True(_sut.Verificar("Sup3rS3creta!", hash));
        Assert.False(_sut.Verificar("otra", hash));
    }

    [Fact]
    public void Dos_hashes_de_la_misma_clave_son_distintos_por_el_salt()
    {
        Assert.NotEqual(_sut.Hashear("igual"), _sut.Hashear("igual"));
    }

    [Fact]
    public void Verifica_password_heredada_en_texto_plano()
    {
        // Valor legacy sin formato PBKDF2.
        Assert.True(_sut.Verificar("bascula", "bascula"));
        Assert.False(_sut.Verificar("mala", "bascula"));
    }

    [Theory]
    [InlineData("bascula", true)]   // texto plano heredado → migrar
    [InlineData("", true)]          // vacío → migrar
    [InlineData(null, true)]
    public void NecesitaRehash_para_valores_heredados(string? almacenada, bool esperado)
    {
        Assert.Equal(esperado, _sut.NecesitaRehash(almacenada));
    }

    [Fact]
    public void No_necesita_rehash_un_hash_actual()
    {
        var hash = _sut.Hashear("clave");
        Assert.False(_sut.NecesitaRehash(hash));
    }
}
