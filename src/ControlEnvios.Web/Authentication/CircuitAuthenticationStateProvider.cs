using System.Security.Claims;
using ControlEnvios.Application.Autenticacion;
using Microsoft.AspNetCore.Components.Authorization;

namespace ControlEnvios.Web.Authentication;

/// <summary>
/// Proveedor de estado de autenticación en memoria del circuito Blazor Server.
/// </summary>
/// <remarks>
/// Suficiente para la navegación por rol en la Fase 5. La persistencia real (cookie de ASP.NET Core,
/// supervivencia a recargas/reconexiones) se implementa en la Fase 6.
/// </remarks>
public sealed class CircuitAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonimo = new(new ClaimsIdentity());
    private ClaimsPrincipal _usuario = Anonimo;

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(_usuario));

    public void IniciarSesion(ResultadoLogin login)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, login.Identificador ?? string.Empty),
            new Claim(ClaimTypes.GivenName, login.Nombre ?? login.Identificador ?? string.Empty),
            new Claim(ClaimTypes.Role, login.Rol!.Value.ToString()),
        ], authenticationType: "Circuit");

        _usuario = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void CerrarSesion()
    {
        _usuario = Anonimo;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
