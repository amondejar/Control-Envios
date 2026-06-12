# Plan de Migración — ControlEnvios

**Origen:** ASP.NET MVC 5 · .NET Framework 4.8 · EF6 (EDMX) · Bootstrap 3 + jQuery + Ext.NET
**Destino:** **Blazor Web App (.NET 10 LTS, interactividad Server)** · **EF Core (Database-First)** · UI renovada con MudBlazor + identidad visual de la empresa
**Rama de trabajo:** `migracion-blazor-net8` (todo el trabajo de migración vive aquí; `main` queda intacta)
**Estado:** Fases 0–2 completadas. En curso la Fase 3.

> **Nota de versión:** el plan se redactó para .NET 8 LTS, pero el equipo de desarrollo solo dispone del
> SDK de **.NET 10** (también LTS, más reciente). El andamiaje se ha generado contra **net10.0**; el enfoque
> (Blazor Server + EF Core database-first) no cambia.

---

## 1. Diagnóstico de la arquitectura actual

### 1.1 Stack y estructura
- **Framework:** ASP.NET MVC 5 sobre **.NET Framework 4.8**, solo Windows/IIS. C# 6 (`/langversion:6`).
- **Proyectos en la solución:**
  - `ControlEnvios` — aplicación web MVC.
  - `SmptEmailSendProveedor` — librería auxiliar de envío de correo (SMTP propio, clases `CSmtp`, `CMensage`, `SendEmail`).
  - `Publicaciones/` — **copia de una publicación** dentro del repo (duplica vistas/recursos). Candidata a eliminar del control de versiones.
- **Módulos funcionales (controladores):**
  - `AccountController` — login de proveedores y usuarios (gestor/producción).
  - `EnviosController` — CRUD de envíos de mercancía, lógica de cupos por proveedor.
  - `GestorController` — vista de gestión.
  - `ProduccionController` — vista de producción.
  - `HomeController`, `ExtNetController`.

### 1.2 Capa de datos
- **Entity Framework 6 con modelo EDMX** (`Model_Bascula_DB.edmx`) — Database-First clásico.
- Base de datos **SQL Server `Bascula`** (`data source=192.168.10.21`).
- Mucho acceso vía **procedimientos almacenados** y SQL crudo: `ENVIOPESADA`, `LISTAENVIOSIDPROVEEDOR`, `KILOS_SEMANA_PROVEEDOR`, etc., a través de `contexto.Database.SqlQuery<T>(...)`.
- Entidades principales: `ENVIOMERCANCIA`, `PROVEEDOR`, `ARTICULO`, `CUPOPROVEEDOR`, `CUPOKG`, `USUARIOS`, `PESADABASCULA`, `MODULO`, `ESTADOMERCANCIA`.

### 1.3 Capa de presentación
- Razor (`.cshtml`) + **mezcla de librerías UI antiguas y solapadas**:
  - Bootstrap 3.0, jQuery 1.10.2, jQuery UI 1.10, **Ext.NET 4.5 (tema Triton)**, jqGrid 4.4, DataTables, Modernizr, Respond.js, Toastr, Moment.js.
- Layout propio (`_Layout.cshtml`, `_Header`, `_Footer`) con CSS a medida (`menus.css`, `simple-sidebar.css`, `framework.css`, `Site.css`).
- JS de aplicación en `Scripts/Application/` (`Envio.js`, `Gestor.js`, `Produccion.js`, `Home.js`, `Login.js`).

### 1.4 Recursos de la empresa (a preservar)
- `images/logo.jpg`, `images/logo_footer.png`, `images/logo_quick_footer.png`, `images/logo_solimon_footer.png`
- `images/background/bgr-home.png`, `images/preload.gif`
- Iconografía en `/Iconos` (`icon-*.png`)
- `favicon.ico`

> Estos activos se migran tal cual a `wwwroot/` en el proyecto destino y se integran en el nuevo diseño.

---

## 2. Deuda técnica y riesgos detectados

| # | Severidad | Hallazgo | Impacto |
|---|-----------|----------|---------|
| 1 | 🔴 Crítica | **Contraseñas en texto plano**: el login compara `PROVEEDOR.PASSWORD == prov.Password` y `USUARIOS.PASSWORD` directos. | Las credenciales se guardan sin hash en BD. Cualquier filtración expone todas las cuentas. |
| 2 | 🔴 Crítica | **Credenciales de BD en claro** en `Web.config` (`user id=Bascula;password=bascula`). | Secreto versionado/desplegado en texto plano. |
| 3 | 🟠 Alta | **Lógica de negocio en controladores** (cálculo de cupos, fechas de semana, acceso a datos directo). Sin capa de servicios ni repositorios. | Imposible de testear, duplicación (bloques `Create` casi idénticos), difícil de mantener. |
| 4 | 🟠 Alta | **Estado en `Session`** (`Session["Proveedor"]`, `EsGestor`, etc.) con objetos de entidad EF completos. | Acoplamiento a sesión de servidor; no escalable; mezcla autenticación (Forms) con estado manual. |
| 5 | 🟠 Alta | **EDMX no existe en .NET moderno** — bloquea cualquier destino .NET 8. | Requiere re-scaffolding obligatorio con EF Core. |
| 6 | 🟡 Media | **Sin inyección de dependencias** — `new BasculaEntities()` instanciado a mano en cada acción; múltiples contextos por petición. | Fugas de recursos, sin gestión de ciclo de vida. |
| 7 | 🟡 Media | **Sin pruebas automatizadas** de ningún tipo. | Migración sin red de seguridad; necesario caracterizar comportamiento. |
| 8 | 🟡 Media | **Librerías UI redundantes y obsoletas** (Ext.NET + jqGrid + DataTables + jQuery UI a la vez). | Peso, conflictos, mantenimiento. Ext.NET es comercial y está descontinuado. |
| 9 | 🟡 Media | Manejo de errores con `catch(Exception)` que vuelca `exc.Message` a la UI vía `TempData`. | Fuga de información interna; UX pobre. |
| 10 | 🟢 Baja | Carpeta `Publicaciones/` y artefactos `obj/bin` en el repo; faltas de ortografía en identificadores (`Elmina`, `Destalles`). | Ruido en el repositorio. |

---

## 3. Arquitectura destino

```
ControlEnvios.sln  (nuevo, .NET 8)
│
├── src/
│   ├── ControlEnvios.Web/            → Blazor Web App (interactividad Server)
│   │     ├── Components/             → Layout, páginas y componentes UI (MudBlazor)
│   │     ├── wwwroot/                → logos, iconos, css de empresa
│   │     └── Program.cs             → DI, auth, EF Core, configuración
│   │
│   ├── ControlEnvios.Application/    → Servicios de negocio + DTOs + interfaces
│   │     (EnvioService, CupoService, AuthService, EmailService...)
│   │
│   ├── ControlEnvios.Domain/         → Entidades/Modelos de dominio, enums
│   │
│   └── ControlEnvios.Infrastructure/ → EF Core DbContext (scaffold), repositorios,
│                                        acceso a procedimientos almacenados, SMTP
│
└── tests/
      ├── ControlEnvios.UnitTests/    → Lógica de cupos, fechas, servicios
      └── ControlEnvios.IntegrationTests/ → Acceso a datos contra BD de pruebas
```

**Decisiones clave:**
- **Blazor Web App, render mode Server interactivo** — sin API pública expuesta, baja latencia contra SQL Server en LAN, reutilización directa de C#.
- **EF Core 8 Database-First** (scaffold con `dotnet ef dbcontext scaffold`) — se conserva el esquema y los procedimientos almacenados existentes; los SP se mapean con `FromSqlRaw`/`SqlQuery`.
- **Inyección de dependencias nativa** — `DbContext` con ciclo de vida correcto, servicios registrados.
- **Configuración por `appsettings.json` + User Secrets / variables de entorno** para la cadena de conexión (fuera del control de versiones).
- **UI: MudBlazor** (Material Design para Blazor) — componentes ricos (DataGrid, formularios, diálogos) que reemplazan Ext.NET + jqGrid + DataTables de un plumazo, con tematización de marca de la empresa.
- **Autenticación:** ASP.NET Core Identity o esquema de cookies + hashing de contraseñas (PBKDF2/`PasswordHasher`). Estrategia de migración de contraseñas heredadas documentada en Fase 6.

---

## 4. Plan por fases

> Cada fase es un conjunto de PRs sobre la rama `migracion-blazor-net8` (o sub-ramas que la integran). La app legacy en `main` sigue operativa hasta el corte final.

### Fase 0 — Preparación del repositorio *(rápida)* ✅ COMPLETADA
- [x] Limpiar el repo: `.gitignore` correcto para .NET; `obj/`, `Publicaciones/` y `.csproj.user` fuera del control de versiones (776 → 302 archivos).
- [x] Documentar el arranque del legacy → [`docs/LEGACY.md`](docs/LEGACY.md).
- [x] Inventario de pantallas y flujos → en `docs/LEGACY.md`. *(Pendiente: capturas — requiere la app en ejecución con BD accesible.)*

### Fase 1 — Caracterización funcional *(red de seguridad)* ✅ COMPLETADA
- [x] Reglas de negocio (cupos, login por rol, cancelación+email, fechas de semana) → [`docs/FASE1-CARACTERIZACION.md`](docs/FASE1-CARACTERIZACION.md).
- [x] Catálogo de procedimientos almacenados con su contrato y discrepancias detectadas.
- [x] Casos de prueba de aceptación por módulo (LOGIN/ENV/GES/PRO).

### Fase 2 — Andamiaje del proyecto destino ✅ COMPLETADA
- [x] Solución `ControlEnvios.Modern.slnx` (.NET 10) con `src/` + `tests/` (Domain, Application, Infrastructure, Web, UnitTests, IntegrationTests) y referencias entre capas.
- [x] DI por capas (`AddApplication`/`AddInfrastructure`), logging Serilog, `appsettings` con secretos en blanco + User Secrets inicializado.
- [x] MudBlazor integrado (providers, layout con AppBar/Drawer, `CompanyTheme` con paleta de empresa). App arranca y sirve HTTP 200.
- [x] CI mínimo en `.github/workflows/ci.yml` (restore + build + test en push/PR de la rama).
- [x] Verificado: `dotnet build` y `dotnet test` correctos; smoke-test de arranque OK.

### Fase 3 — Capa de datos (EF Core)
- [ ] Scaffold del `DbContext` y entidades desde la BD `Bascula` (Database-First).
- [ ] Mapear procedimientos almacenados (`ENVIOPESADA`, `LISTAENVIOSIDPROVEEDOR`, `KILOS_SEMANA_PROVEEDOR`, etc.).
- [ ] Repositorios/servicios de infraestructura con `DbContext` inyectado y ciclo de vida correcto.
- [ ] Pruebas de integración contra una BD de pruebas (no producción).

### Fase 4 — Lógica de negocio (capa Application)
- [ ] Extraer la lógica de los controladores a servicios testeables: `EnvioService`, `CupoService`, `ProveedorService`, `EmailService`.
- [ ] Pruebas unitarias del cálculo de cupos y manejo de fechas (corrigiendo edge cases de semana ISO).
- [ ] Portar `SmptEmailSendProveedor` a un `EmailService` con `MailKit` (o `System.Net.Mail`) configurable.

### Fase 5 — UI renovada (Blazor + MudBlazor), módulo a módulo
- [ ] Layout principal limpio: cabecera con logo de empresa, navegación lateral moderna, footer.
- [ ] Migrar activos de marca (`images/`, `Iconos/`, `favicon`) a `wwwroot/`.
- [ ] **Login** → página Blazor con validación.
- [ ] **Envios** → DataGrid de MudBlazor (reemplaza jqGrid/DataTables), formulario crear/editar en diálogo, detalle.
- [ ] **Gestor** → listados y filtros por fechas/proveedor.
- [ ] **Producción** → equivalente al módulo actual.
- [ ] Notificaciones con Snackbar de MudBlazor (reemplaza Toastr).
- [ ] Diseño responsive y accesible.

### Fase 6 — Seguridad *(crítica, transversal)*
- [ ] **Sacar la cadena de conexión** de la configuración versionada → User Secrets / variables de entorno.
- [ ] **Hashing de contraseñas** (`PasswordHasher`/PBKDF2). Estrategia de transición: re-hash al primer login válido contra el valor heredado, o reseteo coordinado con el cliente.
- [ ] Autenticación basada en cookies de ASP.NET Core + autorización por rol (proveedor / gestor / producción) reemplazando el estado manual en `Session`.
- [ ] Revisión de exposición de mensajes de error; logging estructurado en lugar de volcar `Exception.Message`.

### Fase 7 — Pruebas, paridad y corte
- [ ] Validar paridad funcional contra los casos de la Fase 1 (con el cliente/usuario clave).
- [ ] Pruebas de rendimiento y carga básicas.
- [ ] Plan de despliegue (IIS con ASP.NET Core Hosting Bundle, o Kestrel tras proxy) y rollback.
- [ ] Corte y desmantelamiento del proyecto legacy una vez aceptado.

---

## 5. Riesgos del proyecto y mitigaciones
- **BD de producción compartida:** trabajar siempre contra una **copia/BD de pruebas**; no ejecutar migraciones destructivas. El esquema no se modifica en esta migración.
- **Procedimientos almacenados como caja negra:** documentar su contrato antes de depender de ellos; tests de integración.
- **Contraseñas heredadas sin hash:** requiere decisión del cliente sobre la estrategia de transición (re-hash progresivo vs. reseteo).
- **Ext.NET sin equivalente directo:** MudBlazor cubre la funcionalidad, pero algunas pantallas requerirán rediseño en lugar de port 1:1.

---

## 6. Decisiones pendientes de confirmar con el cliente
1. **Sabor de Blazor:** se propone *Server interactivo*. ¿Hay requisito de funcionamiento offline o de API pública que obligue a WebAssembly? (por defecto: no).
2. **Estrategia de contraseñas heredadas:** ¿re-hash progresivo al login o reseteo coordinado?
3. **Hosting destino:** ¿se mantiene IIS (con Hosting Bundle) o se moderniza (contenedor/Kestrel)?
4. **Biblioteca de componentes:** se propone MudBlazor; alternativas válidas son Radzen o Blazor Bootstrap si se prefiere mantener estética Bootstrap.
5. **Destino de `SmptEmailSendProveedor`:** confirmar parámetros SMTP actuales para portarlos a configuración.

---

*Documento generado en la rama `migracion-blazor-net8`. Próximo paso sugerido: aprobar este plan y comenzar por la Fase 0 (limpieza del repo) y Fase 1 (caracterización funcional).*
