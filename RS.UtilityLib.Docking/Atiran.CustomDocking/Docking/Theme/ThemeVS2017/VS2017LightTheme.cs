namespace Atiran.CustomDocking.Docking.Theme.ThemeVS2017
{

    /// <summary>
    /// Visual Studio 2012 Light theme.
    /// </summary>
    public class VS2017LightTheme : VS2017ThemeBase
    {
        public VS2017LightTheme()
            : base(Decompress(Resources.vs2017Light_vstheme), null, null)
        {
        }
    }
}