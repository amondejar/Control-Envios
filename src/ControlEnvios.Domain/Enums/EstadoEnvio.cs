namespace ControlEnvios.Domain.Enums;

/// <summary>
/// Estado de un envío de mercancía (columna <c>ENVIOMERCANCIA.ESTADO</c>).
/// </summary>
/// <remarks>
/// Catálogo confirmado contra la BD y el flujo legacy (vista de Gestor):
/// <c>1 = Comunicado</c> (alta del proveedor), <c>2 = En Báscula</c>, <c>3 = Pesado</c>,
/// <c>4 = Cancelado</c>.
/// </remarks>
public enum EstadoEnvio
{
    Comunicado = 1,
    EnBascula = 2,
    Pesado = 3,
    Cancelado = 4,
}
