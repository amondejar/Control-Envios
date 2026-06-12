using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControlEnvios.Infrastructure;

/// <summary>
/// Registro de la capa de infraestructura (acceso a datos EF Core, email SMTP).
/// El DbContext y los repositorios se añaden en la Fase 3; el EmailService en la Fase 4.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO Fase 3: registrar BasculaDbContext con la cadena de conexión "BasculaConnection".
        //   var connectionString = configuration.GetConnectionString("BasculaConnection");
        //   services.AddDbContext<BasculaDbContext>(o => o.UseSqlServer(connectionString));
        return services;
    }
}
