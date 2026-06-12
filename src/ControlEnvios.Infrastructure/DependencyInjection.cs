using ControlEnvios.Application.Abstractions.Email;
using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Application.Abstractions.Security;
using ControlEnvios.Infrastructure.Email;
using ControlEnvios.Infrastructure.Persistence;
using ControlEnvios.Infrastructure.Persistence.Repositories;
using ControlEnvios.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControlEnvios.Infrastructure;

/// <summary>
/// Registro de la capa de infraestructura (acceso a datos EF Core, email SMTP, hashing).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Servicios sin dependencia de BD.
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.AddScoped<IEmailService, SmtpEmailService>();

        // DbContext y repositorios se registran siempre, para que el grafo de DI sea completo y la
        // app arranque (la validación del host no instancia el DbContext). La cadena de conexión
        // puede estar vacía mientras no haya acceso a la BD `Bascula`: solo fallará al ejecutar una
        // consulta real, no al arrancar. Se configura por User Secrets / variables de entorno.
        var connectionString = configuration.GetConnectionString("BasculaConnection");
        services.AddDbContext<BasculaDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IArticuloRepository, ArticuloRepository>();
        services.AddScoped<IEnvioRepository, EnvioRepository>();
        services.AddScoped<ICupoRepository, CupoRepository>();
        services.AddScoped<IModuloRepository, ModuloRepository>();

        return services;
    }
}
