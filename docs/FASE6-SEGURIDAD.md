# Fase 6 — Seguridad

Resumen de las medidas de seguridad de la versión migrada y de las acciones pendientes de
coordinación con el cliente. Corrige los hallazgos de
[`FASE1-CARACTERIZACION.md` §4](FASE1-CARACTERIZACION.md#4-hallazgos-de-seguridad-adicionales-para-fase-6).

## 1. Contraseñas — hashing con transición
- **Antes (legacy):** contraseñas en **texto plano** en `PROVEEDOR.PASSWORD` / `USUARIOS.PASSWORD`,
  comparadas directamente.
- **Ahora:** `Pbkdf2PasswordHasher` (PBKDF2-HMAC-SHA256, 210.000 iteraciones, salt de 128 bits).
  Formato almacenado: `PBKDF2$iteraciones$saltB64$hashB64`. Comparación en tiempo constante.
- **Transición sin romper logins:** si el valor almacenado no tiene el formato PBKDF2 se interpreta
  como texto plano heredado y se valida directamente; en ese caso `NecesitaRehash` devuelve `true` y
  `AuthService` **re-hashea y persiste** la contraseña al primer login válido. Migración progresiva,
  transparente para el usuario.
- Cubierto por tests (`Pbkdf2PasswordHasherTests`, `AuthServiceTests`).

## 2. Autenticación — cookie de ASP.NET Core
- **Antes:** Forms Authentication + estado manual en `Session`; además la **cookie se emitía antes de
  validar** las credenciales.
- **Ahora:** esquema de cookie de ASP.NET Core. La UI usa **interactividad Server global** (requisito de
  MudBlazor), por lo que el login es un **formulario nativo** que postea al endpoint `POST /account/login`:
  valida con `AuthService` y solo entonces emite la cookie con `HttpContext.SignInAsync` (claims: nombre,
  rol). Logout en `GET /account/logout` con `SignOutAsync`. La autorización por rol se aplica con
  `[Authorize(Roles=…)]` y `AuthorizeRouteView`; los anónimos se redirigen a `/login`.
- **Opciones de cookie:** `HttpOnly`, `SameSite=Lax`, `SecurePolicy=SameAsRequest`, expiración 8 h con
  expiración deslizante.
- El estado de auth fluye a los componentes interactivos vía `CookieAuthenticationStateProvider`
  (captura el principal de la cookie al iniciar el circuito).

## 3. Secretos fuera del control de versiones
- **Antes:** cadena de conexión con usuario/clave **en claro** en `Web.config`; contraseña SMTP
  **hardcodeada** en `CSmtp.cs` y `App.config`.
- **Ahora:** `appsettings.json` contiene la estructura con los secretos **en blanco**. La cadena de
  conexión (`ConnectionStrings:BasculaConnection`) y la clave SMTP (`Smtp:Password`) se aportan por
  **User Secrets** (desarrollo) o **variables de entorno** (producción). `.gitignore` excluye
  `appsettings.*.local.json` y `*.secrets.json`.

> ⚠️ **Acción pendiente del cliente — rotar credenciales comprometidas.** La clave de la BD `Bascula`
> (`bascula`) y la clave SMTP (`notificaciones@derivadoscitricos.com`) están en el histórico de `main`.
> Deben **rotarse** antes/durante la puesta en producción, ya que su valor original es público en el repo.

## 4. Manejo de errores y logging
- **Antes:** se volcaba `Exception.Message` a la UI vía `TempData`/`Json` (fuga de detalles internos).
- **Ahora:** la UI muestra **mensajes neutros** (snackbars/alertas) y los detalles se registran con
  **Serilog** (logging estructurado). El login no distingue "usuario no existe" de "clave incorrecta".

## 5. Endurecimiento pendiente (recomendado)
- [ ] **Logout por POST** (hoy `/account/logout` es GET; cambiar a POST con antiforgery para evitar
  CSRF de cierre de sesión — severidad baja).
- [ ] **`SecurePolicy=Always`** y HSTS una vez forzado HTTPS en producción.
- [ ] **Bloqueo por intentos fallidos** (rate limiting / lockout) en el login.
- [ ] **Política de contraseñas** y restablecimiento autoservicio (coordinar con el cliente; hoy las
  contraseñas las gestiona la BD heredada).
- [ ] Revisar **autorización a nivel de dato** (que un proveedor solo vea/edite sus propios envíos)
  al conectar la BD.
