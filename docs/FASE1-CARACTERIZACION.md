# Fase 1 — Caracterización funcional (línea base de comportamiento)

Documenta las reglas de negocio, el contrato de los procedimientos almacenados y los casos de
aceptación que la nueva versión (Blazor .NET 8) debe reproducir. Es la **red de seguridad** de la
migración: sirve para validar paridad funcional sin tener tests automatizados en el legacy.

> Referencias: [`../PLAN_MIGRACION.md`](../PLAN_MIGRACION.md) · [`LEGACY.md`](LEGACY.md)

---

## 1. Reglas de negocio

### 1.1 Autenticación y roles (`AccountController`)
Login único con tres caminos de validación, en este orden:
1. **Proveedor** — `PROVEEDOR` con `CODPROVEEDOR == usuario` y `PASSWORD == clave` (texto plano).
   → `Session["Proveedor"]` = entidad; `EsProveedor=true`.
2. **Gestor** — `USUARIOS` con `ALIAS == usuario`, `PASSWORD == clave`, `CHECKWEBADMIN == true`, `ID_PERFIL == 1`.
   → `EsGestor=true`.
3. **Producción** — igual que gestor pero `ID_PERFIL == 5`.
   → `EsProduccion=true`.
- Sin coincidencia → `TempData["Error"] = "Usario o contraseña incorrecta"`.
- Cookie Forms Authentication: expira a **20 min**, o **5 días** si "Recuérdame".
- ⚠️ La cookie de auth se emite **antes** de validar las credenciales contra la BD (se crea siempre en el POST).

### 1.2 Menú por rol (`HomeController`)
Filtra los `MODULO` visibles según el rol (campo `IdModulo`):
| Rol | Módulos excluidos | Visibles |
|-----|-------------------|----------|
| Proveedor | 6, 7 | el resto |
| Gestor | 7, 1 | el resto |
| Producción | 1, 6 | el resto |
- Si `Session["EsGestor"]`/`EsProduccion` es null (sesión caducada) → JSON `"Session caducada."`.

### 1.3 Control de cupos al crear un envío (`EnviosController.Create`) — REGLA CRÍTICA
Al dar de alta un envío de un proveedor:

1. **¿Tiene cupo activo?** `ProvedorTieneCupoActivo()` busca `CUPOPROVEEDOR` por `CODPROVEE == proveedor` de sesión.
   - **Sin cupo activo** (`ACTIVACUPO != true`) → se inserta el envío **sin restricción**.
   - **Con cupo activo** → se aplica el cálculo siguiente.

2. **Datos del cálculo:**
   - `KilosCupoSemana` = `CUPOKG.CUPOKILOS` donde `IDCUPOKG == 1` (cupo global semanal en kg, valor único de la empresa).
   - `PORCENTAGEASIG` = porcentaje asignado al proveedor (`CUPOPROVEEDOR`).
   - `totalKilosEnviadosSemana` = SP `KILOS_SEMANA_PROVEEDOR(lunes, domingo, proveedor)`.
   - `EsPrimerEnvio` = true si el proveedor **no** ha enviado kilos esta semana (total == 0).

3. **Decisión** (`CalculaPorcentageAsignado`):
   - Si **es primer envío de la semana** → **se permite siempre** (aunque exceda).
   - Si **no** es el primero:
     ```
     KilosAsignados = (KilosCupoSemana * PORCENTAGEASIG) / 100
     PermitidoSi:  (kilosEnviadosSemana + kilosNuevoEnvio) <= KilosAsignados
     ```
   - Si no se cumple → `TempData["success"] = "Cupo excedido,por favor contacte con el responsable de compras."` y `RedirectToAction("Index")` (no inserta).

4. **Semana** = lunes a domingo:
   - `ObtenerPrimerdiaSemana()` = `Now.AddDays(-1 * ((int)DayOfWeek - 1))` (lunes).
   - `ObtenerUltimodiaSemana()` = `Now.AddDays(-1 * ((int)DayOfWeek - 7))` (domingo).

**Ejemplo trabajado:** cupo global 100.000 kg, proveedor con `PORCENTAGEASIG = 10` → asignación = 10.000 kg/semana.
Lleva 8.000 kg esta semana y pide 1.500 → 9.500 ≤ 10.000 → **permitido**. Pide 2.500 → 10.500 > 10.000 → **rechazado** (salvo que fuera su primer envío de la semana).

> ⚠️ **Bugs/edge cases a preservar o corregir conscientemente en la migración** (decisión de negocio):
> - **Primer envío sin límite:** un proveedor puede superar su cupo en su primer envío semanal. ¿Intencionado?
> - **NRE potencial:** `ProvedorTieneCupoActivo()` puede devolver `null` y luego se accede a `cupoProv.ACTIVACUPO` (orden de la condición invertido).
> - **Domingo:** el cálculo del lunes con `DayOfWeek` (Domingo=0) da `+1 día`, desplazando la semana. Revisar comportamiento en domingo.
> - **Estado:** `Create` inserta `ESTADO = 1` (Enviado), pero comentarios y `RecuperarEnvios` mencionan `Estado 0`. Confirmar catálogo de estados real en `ESTADOMERCANCIA`.

### 1.4 Consulta por fechas (`GestorController` / `ProduccionController`)
- Filtra envíos por rango `[DtFechaInicio, DtFechaFin]`, opcionalmente por `Proveedor` y/o `Articulo`.
- Gestor cubre las 4 combinaciones (con/sin proveedor × con/sin artículo); Producción solo "todos / por nada".
- Detalle de un envío vía `LISTAENVIOSIDPROVEEDOR`.

### 1.5 Cancelación de envío + email (`GestorController.CancelSendEnvio`)
- Ejecuta SP `CANCELAENVIO(id)` → marca el envío como **estado 4 (Cancelado)**.
- Recarga la lista con `TODOSLOSENVIOSPROVEE` y la filtra por las fechas/proveedor de sesión.
- Obtiene el email del proveedor con `CANCELAENVIOEMAIL(id)` y, si existe, envía un correo de notificación
  (asunto "Cancelación de envío matrícula: …") mediante `SmptEmailSendProveedor`.

> ⚠️ Bugs a tener en cuenta:
> - **Filtro de fecha fin roto:** se guarda `Session["FechaFinGEstor"]` (con typo) pero se lee `Session["FechaFinGestor"]` → el filtro superior por fecha fin queda nulo.
> - **Estado compartido entre usuarios:** `ListaModeloEnvioPorFechas` es `static` en Gestor y Producción → condición de carrera entre sesiones concurrentes. **No replicar** en Blazor.

### 1.6 Envío de correo (`SmptEmailSendProveedor`)
- SMTP de la empresa **Derivados Cítricos**: host `smtp.derivadoscitricos.com`, puerto `25`, sin SSL,
  remitente `notificaciones@derivadoscitricos.com`.
- ⚠️ **La contraseña SMTP está hardcodeada** en `CSmtp.cs` y en `App.config`. Debe moverse a configuración
  segura en la migración (ver hallazgos de seguridad).

---

## 2. Catálogo de procedimientos almacenados

Definidos en el modelo EF (`Model_Bascula_DB.Context.cs`) y/o invocados con `Database.SqlQuery`.

| SP | Parámetros | Devuelve | Usado en |
|----|-----------|----------|----------|
| `ENVIOPESADA` / `PROC_ENVIOPESADA` | `@fechaHoy DATETIME`, `@Proveedor NVARCHAR` | Envíos del proveedor desde hoy (cod/desc artículo, matrícula, fecha/hora, kilos, neto, estado, observaciones) | `Envios.RecuperarEnvios` (raw `ENVIOPESADA`) |
| `LISTAENVIOSIDPROVEEDOR` | `@idEnvio INT` | Detalle de un envío | `Envios.Detail`, `Gestor.Detail`, `Produccion.Detail` |
| `LISTAENVIOSFECHAPROVEEDOR` | `@fechaEnvio`, `@fechaFin`, `@codproveedor`, **`@codArticulo`** | Envíos por rango de fechas (+ filtros) | `Gestor` (4 params), `Produccion` (3 params) |
| `KILOS_SEMANA_PROVEEDOR` | `@fechaInicio`, `@fechaFin`, `@codproveedor` | `DECIMAL?` total kilos de la semana | `Envios` (cupos) |
| `CANCELAENVIO` | `@id INT` | `INT` (filas afectadas); marca estado 4 | `Gestor.CancelSendEnvio` |
| `CANCELAENVIOEMAIL` | `@id INT` | Email + matrícula del proveedor | `Gestor.CancelSendEnvio` |
| `TODOSLOSENVIOSPROVEE` | — | Todos los envíos (para refrescar listado) | `Gestor.CancelSendEnvio` |
| `ALLCUPOSPROVEEDOR` | — | Cupos de todos los proveedores | (declarado, sin uso directo localizado) |
| `RECUPERAENVIOSPROVEE` / `RECUPERAENVIOSPROVEE1` | — | Envíos del proveedor | (declarados, sin uso directo localizado) |

> ⚠️ **Discrepancias de contrato a verificar contra la BD real (Fase 3):**
> - `LISTAENVIOSFECHAPROVEEDOR`: el modelo EF lo declara con **3** parámetros, pero `GestorController`
>   lo llama con **4** (`@codArticulo`). El SP real probablemente tiene 4. Confirmar.
> - `ENVIOPESADA` vs `PROC_ENVIOPESADA`: ¿son el mismo SP con dos nombres? El controlador usa `ENVIOPESADA` por raw SQL.
> - Inconsistencia de nombres de parámetro: `@fechahoy` (raw) vs `fechaHoy` (modelo).

---

## 3. Casos de prueba de aceptación

Numerados para trazabilidad. Se validarán manualmente en el legacy (línea base) y de forma
automatizada en el destino (Fases 4 y 7).

### Login (LOGIN-xx)
- **LOGIN-01** Proveedor válido → entra como proveedor, menú sin módulos 6 y 7.
- **LOGIN-02** Usuario gestor válido (`ID_PERFIL=1`, `CHECKWEBADMIN`) → entra como gestor.
- **LOGIN-03** Usuario producción válido (`ID_PERFIL=5`) → entra como producción.
- **LOGIN-04** Credenciales inválidas → mensaje "Usario o contraseña incorrecta", no entra.
- **LOGIN-05** "Recuérdame" marcado → la sesión persiste tras cerrar el navegador (~5 días).

### Envíos / cupos (ENV-xx)
- **ENV-01** Proveedor sin cupo activo crea envío → se inserta sin restricción.
- **ENV-02** Proveedor con cupo, primer envío de la semana → se permite aunque supere el cupo.
- **ENV-03** Proveedor con cupo, suma semanal ≤ asignación → se permite.
- **ENV-04** Proveedor con cupo, suma semanal > asignación (no primer envío) → "Cupo excedido…", no se inserta.
- **ENV-05** Editar un envío existente → cambios persistidos.
- **ENV-06** Eliminar un envío → desaparece del listado.
- **ENV-07** Validaciones de formulario: hora `HH:mm`, matrícula sin guiones, kilos obligatorios.
- **ENV-08** Detalle de un envío muestra artículo, matrícula, kilos, neto, estado, observaciones.

### Gestor (GES-xx)
- **GES-01** Consulta por rango de fechas sin filtros → lista todos los envíos del rango.
- **GES-02** Consulta filtrando por proveedor / artículo (4 combinaciones).
- **GES-03** Cancelar envío → estado pasa a Cancelado (4) y desaparece/actualiza en la lista.
- **GES-04** Cancelar envío de proveedor con email → se envía correo de notificación.
- **GES-05** Cancelar envío de proveedor sin email → no se envía correo, sin error.

### Producción (PRO-xx)
- **PRO-01** Consulta por rango de fechas → lista de envíos.
- **PRO-02** Detalle de un envío.

---

## 4. Hallazgos de seguridad adicionales (para Fase 6)
Ampliación de los ya listados en el plan:
- 🔴 **Contraseña SMTP hardcodeada** en `SmptEmailSendProveedor/CSmtp.cs` (`ewO83)1d`) y en `App.config`.
- 🔴 **Cookie de autenticación emitida antes de validar credenciales** (`AccountController.Index` POST).
- 🟠 **Estado de sesión compartido en campos `static`** (Gestor/Producción) → fuga de datos entre usuarios.
- 🟠 Entidad EF completa de `PROVEEDOR` (incluida contraseña) almacenada en `Session`.

> Estos secretos (BD y SMTP) están versionados en el histórico de `main`. Tras la migración se recomienda
> **rotar las credenciales** comprometidas (clave de BD `Bascula` y clave SMTP).
