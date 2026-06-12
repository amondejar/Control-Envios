using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Infrastructure.Persistence;
using ControlEnvios.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControlEnvios.Infrastructure;

/// <summary>
/// Registro de la capa de infraestructura (acceso a datos EF Core, email SMTP).
/// El <c>EmailService</c> se añade en la Fase 4.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BasculaConnection");

        // El DbContext solo se registra si hay cadena de conexión configurada (User Secrets / entorno).
        // Mientras no haya acceso a la BD `Bascula`, la app arranca sin la capa de datos activa.
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<BasculaDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProveedorRepository, ProveedorRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IArticuloRepository, ArticuloRepository>();
            services.AddScoped<IEnvioRepository, EnvioRepository>();
            services.AddScoped<ICupoRepository, CupoRepository>();
            services.AddScoped<IModuloRepository, ModuloRepository>();
        }

        return services;
    }
}
