using Microsoft.Extensions.DependencyInjection;

namespace ControlEnvios.Application;

/// <summary>
/// Registro de los servicios de la capa de aplicación (lógica de negocio).
/// Los servicios concretos (EnvioService, CupoService, etc.) se añaden en la Fase 4.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // TODO Fase 4: registrar servicios de negocio.
        return services;
    }
}
