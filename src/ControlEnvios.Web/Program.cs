using System.Security.Claims;
using ControlEnvios.Application;
using ControlEnvios.Application.Autenticacion;
using ControlEnvios.Infrastructure;
using ControlEnvios.Web.Authentication;
using ControlEnvios.Web.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging estructurado con Serilog.
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Componentes de UI (MudBlazor).
builder.Services.AddMudServices();

// Capas de aplicación e infraestructura.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Autenticación / autorización.
// Esquema de cookie con redirección a /login: satisface la autorización del endpoint SSR.
// El estado interactivo lo gobierna el provider del circuito; el inicio de sesión por cookie
// (persistencia entre recargas) se completa en la Fase 6.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.Cookie.Name = "ControlEnvios.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // 'Always' tras HTTPS en producción
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// --- Endpoints de autenticación (firman/borran la cookie; la UI es interactiva global) ---
app.MapPost("/account/login", async (HttpContext http, IAuthService auth) =>
{
    var form = await http.Request.ReadFormAsync();
    var usuario = form["usuario"].ToString();
    var password = form["password"].ToString();
    var returnUrl = UrlLocalSeguro(form["returnUrl"].ToString());

    try
    {
        var r = await auth.AutenticarAsync(usuario, password);
        if (!r.Exito)
        {
            return Results.Redirect($"/login?error=1&returnUrl={Uri.EscapeDataString(returnUrl)}");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, r.Identificador!),
            new Claim(ClaimTypes.GivenName, r.Nombre ?? r.Identificador!),
            new Claim(ClaimTypes.Role, r.Rol!.Value.ToString()),
        };
        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));
        return Results.Redirect(returnUrl);
    }
    catch (Exception)
    {
        return Results.Redirect("/login?error=2");
    }
}).DisableAntiforgery();

// GET por simplicidad desde el menú interactivo (CSRF de logout es de severidad baja; ver FASE6-SEGURIDAD).
app.MapGet("/account/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.Run();

// Evita open-redirect: solo rutas locales.
static string UrlLocalSeguro(string? url) =>
    !string.IsNullOrEmpty(url) && url.StartsWith('/') && !url.StartsWith("//") ? url : "/";
