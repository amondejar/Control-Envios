using ControlEnvios.Domain.Enums;
using MudBlazor;

namespace ControlEnvios.Web.Helpers;

/// <summary>Presentación (texto, color de chip y tinte de fila) para el estado de un envío.</summary>
public static class EstadoEnvioUi
{
    public static string Texto(int estado) => (EstadoEnvio)estado switch
    {
        EstadoEnvio.Comunicado => "Comunicado",
        EstadoEnvio.EnBascula => "En báscula",
        EstadoEnvio.Pesado => "Pesado",
        EstadoEnvio.Cancelado => "Cancelado",
        _ => $"Estado {estado}",
    };

    public static Color Color(int estado) => (EstadoEnvio)estado switch
    {
        EstadoEnvio.Comunicado => MudBlazor.Color.Info,
        EstadoEnvio.EnBascula => MudBlazor.Color.Warning,
        EstadoEnvio.Pesado => MudBlazor.Color.Success,
        EstadoEnvio.Cancelado => MudBlazor.Color.Error,
        _ => MudBlazor.Color.Default,
    };

    /// <summary>Tinte de fondo de fila (tenue) según el estado.</summary>
    public static string FilaEstilo(int estado) => (EstadoEnvio)estado switch
    {
        EstadoEnvio.Pesado => "background-color: rgba(46,125,50,0.08);",
        EstadoEnvio.Cancelado => "background-color: rgba(198,40,40,0.10);",
        EstadoEnvio.EnBascula => "background-color: rgba(255,179,0,0.10);",
        _ => string.Empty,
    };
}
