using MudBlazor;

namespace ControlEnvios.Web.Theming;

/// <summary>
/// Tema de marca de Derivados Cítricos. Paleta base (cítrica) pensada para afinarse
/// en la Fase 5 con los colores exactos del logotipo corporativo.
/// </summary>
public static class CompanyTheme
{
    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#2E7D32",        // verde cítrico
            Secondary = "#F9A825",      // naranja/amarillo
            AppbarBackground = "#2E7D32",
            AppbarText = "#FFFFFF",
            Background = "#F7F9F6",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#2E3A2F",
            Success = "#2E7D32",
            Error = "#C62828",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#66BB6A",
            Secondary = "#FFCA28",
            AppbarBackground = "#1B3A1E",
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = ["Roboto", "Helvetica", "Arial", "sans-serif"] },
        },
    };
}
