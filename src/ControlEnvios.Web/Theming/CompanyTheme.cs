using MudBlazor;

namespace ControlEnvios.Web.Theming;

/// <summary>
/// Tema de marca de Derivados Cítricos: paleta cítrica moderna (verde + naranja/ámbar).
/// Los degradados se aplican vía CSS (ver wwwroot/app.css); aquí se definen los colores base.
/// </summary>
public static class CompanyTheme
{
    // Tonos de marca reutilizados por los degradados CSS.
    public const string Verde = "#2E7D32";
    public const string VerdeOscuro = "#1B5E20";
    public const string Naranja = "#F57C00";
    public const string Ambar = "#FFB300";

    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = Verde,
            Secondary = Naranja,
            Tertiary = Ambar,
            AppbarBackground = Verde,
            AppbarText = "#FFFFFF",
            Background = "#F4F7F2",
            Surface = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#243027",
            DrawerIcon = Verde,
            TextPrimary = "#1F2A22",
            Success = "#2E7D32",
            Warning = Ambar,
            Error = "#C62828",
            Info = "#0277BD",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#66BB6A",
            Secondary = "#FFA726",
            Tertiary = Ambar,
            AppbarBackground = "#15301A",
            Background = "#12161300",
            Surface = "#1E2620",
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "260px",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = ["Roboto", "Segoe UI", "Helvetica", "Arial", "sans-serif"] },
            H4 = new H4Typography { FontWeight = "600" },
            H5 = new H5Typography { FontWeight = "600" },
            H6 = new H6Typography { FontWeight = "600" },
        },
    };
}
