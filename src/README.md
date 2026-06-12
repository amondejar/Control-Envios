# ControlEnvios — Solución moderna (.NET 10 / Blazor)

Destino de la migración del legacy ASP.NET MVC 5. Ver el plan en
[`../PLAN_MIGRACION.md`](../PLAN_MIGRACION.md) y la caracterización funcional en
[`../docs/FASE1-CARACTERIZACION.md`](../docs/FASE1-CARACTERIZACION.md).

> **Nota de versión:** el plan se redactó para .NET 8 LTS, pero este equipo solo tiene el
> SDK de **.NET 10** (también LTS). La solución se ha generado contra **net10.0**.

## Estructura

```
ControlEnvios.Modern.slnx        Solución (formato XML). El legacy sigue en ControlEnvios.sln.
src/
  ControlEnvios.Domain/          Entidades de dominio y enums. Sin dependencias.
  ControlEnvios.Application/     Servicios de negocio, DTOs, interfaces. → Domain
  ControlEnvios.Infrastructure/  EF Core (DbContext, repos) y email SMTP. → Application, Domain
  ControlEnvios.Web/             Blazor Web App (interactividad Server) + MudBlazor. → todas
tests/
  ControlEnvios.UnitTests/       Lógica de negocio (cupos, fechas...). → Application, Domain
  ControlEnvios.IntegrationTests/ Acceso a datos contra BD de pruebas. → Infrastructure
```

## Requisitos
- SDK de **.NET 10**.

## Compilar y probar
```bash
dotnet build ControlEnvios.Modern.slnx
dotnet test  ControlEnvios.Modern.slnx
dotnet run --project src/ControlEnvios.Web
```

## Configuración y secretos
Los secretos **no se versionan**. La cadena de conexión y la contraseña SMTP se
proporcionan por User Secrets (desarrollo) o variables de entorno (producción):

```bash
dotnet user-secrets set "ConnectionStrings:BasculaConnection" "Server=...;Database=Bascula;User Id=...;Password=...;TrustServerCertificate=True" --project src/ControlEnvios.Web
dotnet user-secrets set "Smtp:Password" "***" --project src/ControlEnvios.Web
```

`appsettings.json` contiene la estructura con los valores no sensibles y los secretos en blanco.

## Estado de la migración
- ✅ Fase 0 — limpieza del repositorio.
- ✅ Fase 1 — caracterización funcional.
- ✅ Fase 2 — andamiaje (esta solución): capas, DI, Serilog, MudBlazor + tema de empresa, CI.
- ✅ Fase 3 — entidades de dominio + persistencia EF Core "en seco" (DbContext, Fluent API, repos). *Scaffold real y SPs pendientes de acceso a la BD.*
- ✅ Fase 4 — servicios de negocio (cupos, auth, envíos, email) con 32+ tests.
- ✅ Fase 5 — UI Blazor + MudBlazor por módulo (login, envíos, gestor, producción) con la marca de empresa.
- ✅ Fase 6 — seguridad: hashing PBKDF2 con transición, autenticación por cookie, secretos fuera del repo.
- ⏭️ Fase 7 — paridad, despliegue y corte *(requiere acceso a la BD)*.
