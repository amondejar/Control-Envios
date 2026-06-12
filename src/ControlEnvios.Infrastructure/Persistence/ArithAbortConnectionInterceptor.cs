using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ControlEnvios.Infrastructure.Persistence;

/// <summary>
/// Ejecuta <c>SET ARITHABORT ON</c> al abrir cada conexión. <c>Microsoft.Data.SqlClient</c> usa
/// <c>ARITHABORT OFF</c> por defecto (a diferencia de SSMS/sqlcmd), lo que genera planes distintos y
/// degrada gravemente algunos procedimientos legacy (p. ej. <c>LISTAENVIOSFECHAPROVEEDOR</c>, que pasa
/// de instantáneo a ~55 s). Esto lo evita sin tocar la BD ni los SPs.
/// </summary>
internal sealed class ArithAbortConnectionInterceptor : DbConnectionInterceptor
{
    private const string Comando = "SET ARITHABORT ON;";

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = Comando;
        cmd.ExecuteNonQuery();
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = Comando;
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
