using ControlEnvios.Domain.Entities;
using ControlEnvios.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlEnvios.Infrastructure.Persistence.Configurations;

// Mapeo a las columnas reales del esquema legacy "Bascula". Ver docs/FASE3-MAPEO-ENTIDADES.md.
// Las longitudes/precisiones exactas se ajustarán al validar contra la BD real.

public sealed class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> b)
    {
        b.ToTable("PROVEEDOR");
        b.HasKey(x => x.Codigo);
        b.Property(x => x.Codigo).HasColumnName("CODPROVEEDOR");
        b.Property(x => x.Nombre).HasColumnName("NOMBRE");
        b.Property(x => x.Cif).HasColumnName("CIF");
        b.Property(x => x.Direccion).HasColumnName("DIRECCION");
        b.Property(x => x.Poblacion).HasColumnName("POBLACION");
        b.Property(x => x.Provincia).HasColumnName("PROVINCIA");
        b.Property(x => x.CodigoPostal).HasColumnName("CODPOSTAL");
        b.Property(x => x.Observaciones).HasColumnName("OBSERVACIONES");
        b.Property(x => x.Email).HasColumnName("EMAIL");
        b.Property(x => x.Password).HasColumnName("PASSWORD");
    }
}

public sealed class EnvioConfiguration : IEntityTypeConfiguration<Envio>
{
    public void Configure(EntityTypeBuilder<Envio> b)
    {
        b.ToTable("ENVIOMERCANCIA");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("IDENVIO");
        b.Property(x => x.CodigoProveedor).HasColumnName("CODPROVEEDOR");
        b.Property(x => x.CodigoArticulo).HasColumnName("CODARTICULO");
        b.Property(x => x.FechaEnvio).HasColumnName("FECHAENVIO")
            .HasConversion(new DateOnlyToDateTimeConverter()).HasColumnType("datetime");
        b.Property(x => x.HoraEnvio).HasColumnName("HORAENVIO")
            .HasConversion(new TimeOnlyToDateTimeConverter()).HasColumnType("datetime");
        b.Property(x => x.KilosEnviados).HasColumnName("KILOSENVIADOS");
        b.Property(x => x.Estado).HasColumnName("ESTADO"); // enum -> int
        b.Property(x => x.Matricula).HasColumnName("MATRICULA");
        b.Property(x => x.Observaciones).HasColumnName("OBSERVACIONES");
        b.Property(x => x.EnviarEmail).HasColumnName("SENDEMAIL");
        b.Property(x => x.TieneNeto).HasColumnName("TIENENETO");
    }
}

public sealed class ArticuloConfiguration : IEntityTypeConfiguration<Articulo>
{
    public void Configure(EntityTypeBuilder<Articulo> b)
    {
        b.ToTable("ARTICULO");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ID_ARTICULO");
        b.Property(x => x.Codigo).HasColumnName("CODARTICULO");
        b.Property(x => x.Descripcion).HasColumnName("DESCRIPCION");
        b.Property(x => x.ActivoWeb).HasColumnName("ACTIVAWEB");
    }
}

public sealed class CupoProveedorConfiguration : IEntityTypeConfiguration<CupoProveedor>
{
    public void Configure(EntityTypeBuilder<CupoProveedor> b)
    {
        b.ToTable("CUPOPROVEEDOR");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("IDCUPOPROV");
        b.Property(x => x.CodigoProveedor).HasColumnName("CODPROVEE");
        b.Property(x => x.CodigoArticulo).HasColumnName("CODARTICULO");
        b.Property(x => x.FechaInicio).HasColumnName("FECHAINICIOCUPO")
            .HasConversion(new DateOnlyToDateTimeConverter()).HasColumnType("datetime");
        b.Property(x => x.FechaFin).HasColumnName("FEHAFINALCUPO") // typo del esquema legacy (intencionado)
            .HasConversion(new DateOnlyToDateTimeConverter()).HasColumnType("datetime");
        b.Property(x => x.PorcentajeAsignado).HasColumnName("PORCENTAGEASIG");
        b.Property(x => x.Activo).HasColumnName("ACTIVACUPO");
    }
}

public sealed class CupoGlobalConfiguration : IEntityTypeConfiguration<CupoGlobal>
{
    public void Configure(EntityTypeBuilder<CupoGlobal> b)
    {
        b.ToTable("CUPOKG");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("IDCUPOKG");
        b.Property(x => x.CupoKilos).HasColumnName("CUPOKILOS");
    }
}

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("USUARIOS");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ID_USUARIO");
        b.Property(x => x.Perfil).HasColumnName("ID_PERFIL"); // enum -> int
        b.Property(x => x.Nombre).HasColumnName("NOMBRE");
        b.Property(x => x.Alias).HasColumnName("ALIAS");
        b.Property(x => x.Password).HasColumnName("PASSWORD");
        b.Property(x => x.AccesoWebAdmin).HasColumnName("CHECKWEBADMIN");
    }
}

public sealed class ModuloConfiguration : IEntityTypeConfiguration<Modulo>
{
    public void Configure(EntityTypeBuilder<Modulo> b)
    {
        b.ToTable("MODULO");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("IdModulo");
        b.Property(x => x.Nombre).HasColumnName("NombreModulo");
        b.Property(x => x.Accion).HasColumnName("Acction"); // typo del esquema legacy (intencionado)
        b.Property(x => x.Controlador).HasColumnName("Controller");
        b.Property(x => x.Icono).HasColumnName("icono");
        b.Property(x => x.FondoNombre).HasColumnName("bgName");
    }
}

public sealed class EstadoMercanciaConfiguration : IEntityTypeConfiguration<EstadoMercancia>
{
    public void Configure(EntityTypeBuilder<EstadoMercancia> b)
    {
        b.ToTable("ESTADOMERCANCIA");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ID");
        b.Property(x => x.Nombre).HasColumnName("ESTADOMERCANCIA");
    }
}

public sealed class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> b)
    {
        b.ToTable("NOTIFICACION");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Id");
        b.Property(x => x.CodigoProveedor).HasColumnName("CodProveedor");
        b.Property(x => x.Tipo).HasColumnName("Tipo");
        b.Property(x => x.Titulo).HasColumnName("Titulo");
        b.Property(x => x.Mensaje).HasColumnName("Mensaje");
        b.Property(x => x.IdEnvio).HasColumnName("IdEnvio");
        b.Property(x => x.FechaCreacion).HasColumnName("FechaCreacion");
        b.Property(x => x.Leida).HasColumnName("Leida");
        b.Property(x => x.FechaLeida).HasColumnName("FechaLeida");
    }
}

public sealed class PesadaBasculaConfiguration : IEntityTypeConfiguration<PesadaBascula>
{
    public void Configure(EntityTypeBuilder<PesadaBascula> b)
    {
        b.ToTable("PESADABASCULA");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ID_PESADA");
        b.Property(x => x.IdTicket).HasColumnName("ID_TICKET");
        b.Property(x => x.Matricula).HasColumnName("MATRICULA");
        b.Property(x => x.Remolque).HasColumnName("REMOLQUE");
        b.Property(x => x.CodigoProveedor).HasColumnName("COD_PROV");
        b.Property(x => x.CodigoCliente).HasColumnName("COD_CLIE");
        b.Property(x => x.CodigoArticulo).HasColumnName("COD_ART");
        b.Property(x => x.PrecioKg).HasColumnName("PRECIOKG");
        b.Property(x => x.FechaEntrada).HasColumnName("FECHAENTRADA");
        b.Property(x => x.HoraEntrada).HasColumnName("HORAENTRADA");
        b.Property(x => x.FechaSalida).HasColumnName("FECHASALIDA");
        b.Property(x => x.HoraSalida).HasColumnName("HORASALIDA");
        b.Property(x => x.Pesada1).HasColumnName("PESADA1");
        b.Property(x => x.Pesada2).HasColumnName("PESADA2");
        b.Property(x => x.PesadaNeto).HasColumnName("PESADANETO");
        b.Property(x => x.PrimeraPesada).HasColumnName("PRIMEPESADA");
        b.Property(x => x.SegundaPesada).HasColumnName("SECUNPESADA");
        b.Property(x => x.Observaciones).HasColumnName("OBSERVACIONES");
        b.Property(x => x.UsuarioPesada).HasColumnName("USERPESADA");
        b.Property(x => x.InfoModificacionPesada).HasColumnName("INFOMODPESADA");
        b.Property(x => x.EstadoCamion).HasColumnName("ESTADOCAMION");
        b.Property(x => x.EstadoMercancia).HasColumnName("ESTADOMERCANCIA");
        b.Property(x => x.BasculaGrande).HasColumnName("BALSAGRANDE");
        b.Property(x => x.BasculaPequena).HasColumnName("BALSAPEQUENA");
        b.Property(x => x.IdEnvioWeb).HasColumnName("IDENVIOWEB");
        b.Property(x => x.EmailEnviado).HasColumnName("EMAILENVIADO");
        b.Property(x => x.VgmTara).HasColumnName("VGMTARA");
    }
}
