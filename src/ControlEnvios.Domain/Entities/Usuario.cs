using ControlEnvios.Domain.Enums;

namespace ControlEnvios.Domain.Entities;

/// <summary>Usuario interno (gestor / producción). Tabla legacy <c>USUARIOS</c> (PK <c>ID_USUARIO</c>).</summary>
public class Usuario
{
    /// <summary>Clave primaria. (<c>ID_USUARIO</c>)</summary>
    public int Id { get; set; }

    /// <summary>Perfil del usuario. (<c>ID_PERFIL</c>)</summary>
    public PerfilUsuario Perfil { get; set; }

    public string? Nombre { get; set; }

    /// <summary>Alias con el que inicia sesión. (<c>ALIAS</c>)</summary>
    public required string Alias { get; set; }

    /// <summary>
    /// Contraseña en texto plano en el legacy (<c>PASSWORD</c>). Se sustituye por hash en la Fase 6.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>Si el usuario puede acceder a la administración web. (<c>CHECKWEBADMIN</c>)</summary>
    public bool AccesoWebAdmin { get; set; }
}
