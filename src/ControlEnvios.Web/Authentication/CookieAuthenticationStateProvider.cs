using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ControlEnvios.Web.Authentication;

/// <summary>
/// Proveedor de estado de autenticación que captura el usuario de la cookie (vía
/// <see cref="IHttpContextAccessor"/>) al iniciar el circuito y lo mantiene durante su vida.
/// </summary>
/// <remarks>
/// El <see cref="IHttpContextAccessor.HttpContext"/> está disponible en la solicitud inicial del
/// circuito; se captura una vez (ámbito scoped) para que las re-renderizaciones interactivas
/// posteriores conserven el principal aunque ya no haya <c>HttpContext</c>.
/// </remarks>
public sealed class CookieAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly Task<AuthenticationState> _estado;

    public CookieAuthenticationStateProvider(IHttpContextAccessor accessor)
    {
        var usuario = accessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
        _estado = Task.FromResult(new AuthenticationState(usuario));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _estado;
}
