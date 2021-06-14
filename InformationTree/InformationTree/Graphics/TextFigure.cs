using System;
using D = System.Drawing;

namespace InformationTree.Graphics
{
    public class TextFigure : BaseFigure
    {
        #region Properties

        #region Private

        private string fontFamilyName;
        private int blue;
        private int green;
        private int red;
        private double size;

        #endregion Private

        #region Public

        public override FigureType FigureType
        {
            get { return FigureType.Text; }
        }
        public string Text { get; set; }
        public int Level { get; private set; } // tree level


        #endregion Public

        #endregion Properties

        #region Constructors

        public TextFigure(string _text, int _level) : this(_text, 0, 0, 255, 255, 255) { Level = _level;  }
        public TextFigure(string _text, int _x, int _y) : this(_text, _x, _y, 255, 255, 255) { }
        public TextFigure(string _text, int _x, int _y, int _r, int _g, int _b) : this(_text, _x, _y, "AngelicWar.ttf", 12, _r, _g, _b) { }
        public TextFigure(string _text, int _x, int _y, string _fontFamilyName, double _size, int _r, int _g, int _b)
        {
            SetText(_text);
            Move(_x, _y);
            SetFont(_fontFamilyName, _size);
            SetColor(_r, _g, _b);
        }

        #endregion Constructors

        #region Methods

        public override void Show(D.Graphics graphics)
        {
            var point = new D.PointF((float)X, (float)Y);
            var color = (red == 255 && green == 255 && blue == 255) ? D.Color.LightGreen : D.Color.FromArgb(red, green, blue);
            var brush = new D.SolidBrush(color);
            var font = new D.Font(fontFamilyName, (float)size);
            graphics.DrawString(Text.Replace('_', ' '), font, brush, point);
        }

        public void SetFont(string _fontFamilyName, double _size)
        {
            fontFamilyName = _fontFamilyName;
            size = _size;
        }

        public void SetText(string _text)
        {
            Text = _text;
        }

        public void SetText(string t, int x, int y) //text
        {
            Text = t;
            X = x;
            Y = y;
        }

        public void SetColor(int _r, int _g, int _b)
        {
            red = _r;
            blue = _g;
            green = _b;
        }

        public override string GetDebugText()
        {
            return "Text=" + Text.ToString() + ";";
        }

        #endregion Methods
    }
}