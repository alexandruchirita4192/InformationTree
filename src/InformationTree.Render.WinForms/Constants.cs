using System.Drawing;

namespace InformationTree.Render.WinForms
{
    public static class WinFormsConstants
    {
        public static class FontDefaults
        {
            public const float DefaultFontSize = 8.5F;

            public static readonly FontFamily DefaultFontFamily = FontFamily.GenericSansSerif;

            public static readonly Font DefaultFont = new Font(DefaultFontFamily, DefaultFontSize, FontStyle.Regular);
        }
    }
}