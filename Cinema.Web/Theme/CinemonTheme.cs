using MudBlazor;

namespace Cinemon.Web.Theme
{
    public static class CinemonTheme
    {
        public static MudTheme Default => new()
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#8B0000",
                Secondary = "#D4AF37",
                Background = "#F5F5F5",
                Surface = "#FFFFFF"
            },

            PaletteDark = new PaletteDark
            {
                Primary = "#B71C1C",
                Secondary = "#D4AF37",
                Background = "#121212",
                Surface = "#1E1E1E"
            },

            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "8px"
            }
        };
    }
}
