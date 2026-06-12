namespace ControlEnvios.Domain.Enums;

/// <summary>
/// Perfil de un usuario interno (columna <c>USUARIOS.ID_PERFIL</c>).
/// </summary>
/// <remarks>
/// Valores confirmados en <c>AccountController</c>: <c>Gestor = 1</c> y <c>Produccion = 5</c>.
/// Pueden existir otros perfiles en la BD; verificar en la Fase 3.
/// </remarks>
public enum PerfilUsuario
{
    Gestor = 1,
    Produccion = 5,
}
