using ControlEnvios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.Infrastructure.Persistence;

/// <summary>
/// Contexto EF Core sobre la base de datos legacy <c>Bascula</c> (Database-First).
/// El esquema no se modifica: el mapeo a las columnas reales se hace por Fluent API en
/// <c>Persistence/Configurations</c>. Pendiente de validar contra la BD real (Fase 3).
/// </summary>
public class BasculaDbContext : DbContext
{
    public BasculaDbContext(DbContextOptions<BasculaDbContext> options) : base(options)
    {
    }

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Envio> Envios => Set<Envio>();
    public DbSet<Articulo> Articulos => Set<Articulo>();
    public DbSet<CupoProveedor> CuposProveedor => Set<CupoProveedor>();
    public DbSet<CupoGlobal> CupoGlobal => Set<CupoGlobal>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Modulo> Modulos => Set<Modulo>();
    public DbSet<EstadoMercancia> EstadosMercancia => Set<EstadoMercancia>();
    public DbSet<PesadaBascula> PesadasBascula => Set<PesadaBascula>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BasculaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
