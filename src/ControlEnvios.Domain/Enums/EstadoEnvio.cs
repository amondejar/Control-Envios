namespace ControlEnvios.Domain.Enums;

/// <summary>
/// Estado de un envío de mercancía (columna <c>ENVIOMERCANCIA.ESTADO</c>).
/// </summary>
/// <remarks>
/// Valores confirmados en el código legacy: <c>Enviado = 1</c> (alta en <c>EnviosController.Create</c>)
/// y <c>Cancelado = 4</c> (SP <c>CANCELAENVIO</c>). El catálogo completo vive en la tabla
/// <c>ESTADOMERCANCIA</c>; verificar y completar los valores intermedios en la Fase 3 con acceso a la BD.
/// </remarks>
public enum EstadoEnvio
{
    Enviado = 1,
    Cancelado = 4,
}
