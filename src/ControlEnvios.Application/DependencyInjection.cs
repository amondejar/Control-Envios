using ControlEnvios.Application.Autenticacion;
using ControlEnvios.Application.Cupos;
using ControlEnvios.Application.Envios;
using Microsoft.Extensions.DependencyInjection;

namespace ControlEnvios.Application;

/// <summary>
/// Registro de los servicios de la capa de aplicación (lógica de negocio).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingletonTimeProvider();

        services.AddScoped<ICupoService, CupoService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEnvioService, EnvioService>();
        return services;
    }

    private static void TryAddSingletonTimeProvider(this IServiceCollection services)
    {
        if (!services.Any(d => d.ServiceType == typeof(TimeProvider)))
        {
            services.AddSingleton(TimeProvider.System);
        }
    }
}
