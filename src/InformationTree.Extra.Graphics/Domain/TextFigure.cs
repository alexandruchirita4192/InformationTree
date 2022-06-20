using InformationTree.Domain;
using D = System.Drawing;

namespace InformationTree.Extra.Graphics.Domain
{
    public class TextFigure : BaseFigure
    {
        #region Properties

        #region Private

        private double _size;
        private string _fontFamilyName;
        private int _blue;
        private int _green;
        private int _red;

        #endregion Private

        #region Public

        public override FigureType FigureType
        {
            get { return FigureType.Text; }
        }

        public string Text { get; set; }

        #endregion Public

        #endregion Properties

        #region Constructors

        public TextFigure(string text, double x, double y) : this(text, x, y, 255, 255, 255)
        {
        }

        public TextFigure(string text, double x, double y, int red, int green, int blue) : this(text, x, y, "AngelicWar.ttf", 12, red, green, blue)
        {
        }

        public TextFigure(string text, double x, double y, string fontFamilyName, double size, int red, int green, int blue)
        {
            Text = text;
            Move(x, y);
            _fontFamilyName = fontFamilyName;
            _size = size;
            SetColor(red, green, blue);
        }

        #endregion Constructors

        #region Methods

        public override void Show(D.Graphics graphics)
        {
            var point = new PointF((float)X, (float)Y);
            var color = _red == 255 && _green == 255 && _blue == 255 ? Color.LightGreen : Color.FromArgb(_red, _green, _blue);
            var brush = new SolidBrush(color);
            var font = new Font(_fontFamilyName, (float)_size);
            var text = Text?.Replace(Constants.Parsing.UnderscoreSeparator, Constants.Parsing.SpaceSeparator);
            graphics.DrawString(text, font, brush, point);
        }

        public void SetFont(string fontFamilyName, double size)
        {
            _fontFamilyName = fontFamilyName;
            _size = size;
        }

        public void SetColor(int red, int green, int blue)
        {
            _red = red;
            _blue = green;
            _green = blue;
        }

        public override string GetDebugText()
        {
            return $"Text={Text};";
        }

        #endregion Methods
    }
}