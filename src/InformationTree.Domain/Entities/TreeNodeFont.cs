using InformationTree.Domain.Extensions;

namespace InformationTree.Domain.Entities
{
    public class TreeNodeFont
    {
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Underline { get; set; }
        public bool Strikeout { get; set; }
        public float Size { get; set; }
        public string FontFamilyName { get; set; }
        public bool IsEmpty => !Bold && !Italic && !Underline && !Strikeout && Size <= 0 && FontFamilyName.IsEmpty();
        
        public TreeNodeFont Clone()
        {
            var currentFont = this;
            var newFont = new TreeNodeFont
            {
                Bold = currentFont.Bold,
                Italic = currentFont.Italic,
                Underline = currentFont.Underline,
                Strikeout = currentFont.Strikeout,
                Size = currentFont.Size,
                FontFamilyName = currentFont.FontFamilyName
            };
            return newFont;
        }
    }
}