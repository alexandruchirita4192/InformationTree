using System.Drawing;

namespace InformationTree.Render.WinForms
{
    public static class WinFormsConstants
    {
        public static class FontDefaults
        {
            public const float DefaultFontSize = 9F;

            public static readonly FontFamily DefaultFontFamily = FontFamily.GenericSansSerif;

            public static readonly string DefaultFontFamilyName = DefaultFontFamily.Name;

            public static readonly Font DefaultFont = new Font(DefaultFontFamily, DefaultFontSize, FontStyle.Regular);
        }
    }
}