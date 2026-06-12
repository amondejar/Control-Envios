# Aplicación Legacy — Referencia (ControlEnvios)

Documento de referencia del sistema **actual** (rama `main`), para tener una línea base de
comportamiento durante la migración a .NET 8 / Blazor. No describe el destino, solo lo que existe hoy.

> Ver el plan de migración en [`../PLAN_MIGRACION.md`](../PLAN_MIGRACION.md).

---

## 1. Requisitos para compilar y ejecutar

- **Windows** con **IIS Express** (la app es solo .NET Framework).
- **Visual Studio 2019/2022** con la carga de trabajo *ASP.NET y desarrollo web*.
- **.NET Framework 4.8** Developer Pack.
- Acceso de red a la base de datos **SQL Server `Bascula`** (`data source=192.168.10.21`).

### Pasos
1. Abrir `ControlEnvios.sln` en Visual Studio.
2. Restaurar paquetes NuGet (clic derecho en la solución → *Restaurar paquetes NuGet*). La carpeta
   `packages/` ya **no** está versionada; se recupera con el restore.
3. Compilar la solución (`Ctrl+Shift+B`).
4. Ejecutar (`F5`). IIS Express sirve en **http://localhost:2281/** (configurado en el `.csproj`).

> Nota: la cadena de conexión está en `ControlEnvios/Web.config` (`<connectionStrings>`), hoy con
> credenciales en texto plano. En la migración se mueve a configuración fuera del control de versiones.

---

## 2. Proyectos de la solución

| Proyecto | Tipo | Descripción |
|----------|------|-------------|
| `ControlEnvios` | ASP.NET MVC 5 (.NET FW 4.8) | Aplicación web principal. |
| `SmptEmailSendProveedor` | Class Library | Envío de correo SMTP a proveedores (`CSmtp`, `CMensage`, `SendEmail`). |
| `Publicaciones/` (carpeta, no proyecto) | — | Copia de una publicación. **Ya no versionada** (artefacto de despliegue). |

---

## 3. Inventario de pantallas y flujos

> Pendiente: añadir capturas de cada pantalla como línea base visual de UX.

### Autenticación — `AccountController`
- **`/Account/Index`** — Login único para tres tipos de usuario:
  - **Proveedor:** valida contra `PROVEEDOR` (CODPROVEEDOR + PASSWORD). Guarda el proveedor en `Session["Proveedor"]`.
  - **Gestor:** valida contra `USUARIOS` con `ID_PERFIL == 1` y `CHECKWEBADMIN`.
  - **Producción:** valida contra `USUARIOS` con `ID_PERFIL == 5` y `CHECKWEBADMIN`.
  - Usa Forms Authentication (cookie) + estado manual en `Session` (`EsProveedor`, `EsGestor`, `EsProduccion`).
  - "Recuérdame" extiende la expiración del ticket a 5 días.

### Inicio — `HomeController`
- **`/Home/Index`** — Portada tras login; el contenido cambia según el rol.

### Envíos (rol Proveedor) — `EnviosController`
- **`/Envios/Index`** — Listado de envíos del proveedor.
- **`/Envios/Create`** (GET/POST) — Alta de envío de mercancía. Aplica **control de cupos**:
  - Si el proveedor tiene `CUPOPROVEEDOR.ACTIVACUPO`, calcula el porcentaje semanal asignado
    (`CalculaPorcentageAsignado`, `KilosEnviadosPorveedorSemana`, `DameKilosCupoSemana`) y bloquea si se excede.
- **`/Envios/Edit`** (GET/POST) — Edición de un envío existente.
- **`/Envios/Delete`** — Eliminación de un envío.
- **`/Envios/Detail`** — Detalle de un envío (vía SP `LISTAENVIOSIDPROVEEDOR`).
- **`/Envios/RecuperarEnvios`** — Envíos desde hoy en adelante (vía SP `ENVIOPESADA`).

### Gestor (rol Gestor) — `GestorController`
- **`/Gestor/Index`** + parciales `_IntroDatos`, `_DetalleGestor`, `_DestallesEnviosFechas` — Consulta y
  gestión de envíos por proveedor / rango de fechas.

### Producción (rol Producción) — `ProduccionController`
- **`/Produccion/Index`** + parciales `_IntroDatos`, `_DetalleProduccion`, `_DestallesEnviosFechas` —
  Vista equivalente para el perfil de producción.

### `ExtNetController`
- **`/ExtNet/Index`** — Pantalla basada en componentes Ext.NET.

---

## 4. Procedimientos almacenados usados (contrato a documentar en Fase 1)

| SP | Usado en | Parámetros (observados) |
|----|----------|--------------------------|
| `ENVIOPESADA` | `EnviosController.RecuperarEnvios` | `@fechahoy`, `@Proveedor` |
| `LISTAENVIOSIDPROVEEDOR` | `EnviosController.Detail` | `@id` |
| `KILOS_SEMANA_PROVEEDOR` | `EnviosController` (cupos) | fecha inicio, fecha fin, codProveedor |

Otros resultados mapeados en el modelo: `ALLCUPOSPROVEEDOR_Result`, `CANCELAENVIOEMAIL_Result`,
`LISTAENVIOSFECHAPROVEEDOR_Result`, `RECUPERAENVIOSPROVEE_Result`, `TODOSLOSENVIOSPROVEE_Result`,
`PROC_ENVIOPESADA_Result` — revisar y catalogar en la Fase 1.

---

## 5. Entidades principales (BD `Bascula`)

`ENVIOMERCANCIA`, `PROVEEDOR`, `ARTICULO`, `CUPOPROVEEDOR`, `CUPOKG`, `USUARIOS`, `PESADABASCULA`,
`MODULO`, `ESTADOMERCANCIA`.
