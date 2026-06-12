using System.Data;
using ControlEnvios.Application.Consultas;
using ControlEnvios.Infrastructure.Persistence;
using ControlEnvios.Infrastructure.Persistence.Consultas;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.Infrastructure.Consultas;

internal sealed class ConsultaEnviosService(BasculaDbContext context) : IConsultaEnviosService
{
    public async Task<IReadOnlyList<ConsultaEnvio>> PorFechasAsync(
        DateOnly inicio, DateOnly fin, string? codigoProveedor, string? codigoArticulo, CancellationToken ct = default)
    {
        // Tipos EXPLÍCITOS que coinciden con la firma del SP (datetime, varchar(10)). Si se dejan inferir,
        // EF envía datetime2/nvarchar y las conversiones implícitas degradan el plan (timeout).
        var parametros = new[]
        {
            new SqlParameter("@fechaEnvio", SqlDbType.DateTime) { Value = inicio.ToDateTime(TimeOnly.MinValue) },
            new SqlParameter("@fechaFin", SqlDbType.DateTime) { Value = fin.ToDateTime(new TimeOnly(23, 59, 59)) },
            new SqlParameter("@codproveedor", SqlDbType.VarChar, 10) { Value = codigoProveedor ?? string.Empty },
            new SqlParameter("@codArticulo", SqlDbType.VarChar, 10) { Value = codigoArticulo ?? string.Empty },
        };

        var filas = await context.Set<EnvioConsultaResultado>()
            .FromSqlRaw("EXEC LISTAENVIOSFECHAPROVEEDOR @fechaEnvio, @fechaFin, @codproveedor, @codArticulo", parametros)
            .ToListAsync(ct);

        return filas.Select(f => new ConsultaEnvio
        {
            Id = f.Id,
            NombreArticulo = f.NombreArticulo,
            Matricula = f.Matricula,
            CodigoProveedor = f.CodProveedor,
            NombreProveedor = f.NomProveedor,
            FechaEnvio = f.FechaEnvio,
            HoraEnvio = f.HoraEnvio,
            KilosEnviados = f.KilosEnviados,
            Estado = f.Estado,
            PesoBascula = f.PesoBascula,
            BasculaGrande = f.BasculaG,
            BasculaPequena = f.BasculaP,
            HoraSalida = f.HoraSalida,
            Observaciones = f.Observaciones,
        }).ToList();
    }
}
