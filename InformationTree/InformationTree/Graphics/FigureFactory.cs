namespace InformationTree.Graphics
{
    public static class FigureFactory // TODO: Should this be a builder instead of "factory"?
    {
        public static BaseFigure GetFigure(string _line, bool isTextOnly = false)
        {
            BaseFigure ret = null;
            int _x, _y, _r, _g, _b, _level, Points;
            double X, Y, R, Rotation;

            var words = _line.Split(' ');
            switch (words.Length)
            {
                case 2: //Figure(string _text, int _level)
                    // Text = words[0]
                    _level = int.Parse(words[1]);

                    ret = new TextFigure(words[0], _level);
                    break;

                case 3: //Figure(string _text, int _x, int _y)
                    // Text = words[0]
                    _x = int.Parse(words[1]);
                    _y = int.Parse(words[2]);

                    ret = new TextFigure(words[0], _x, _y);
                    break;

                case 4: //Figure(int _points, double _x, double _y, double _r)
                    if (isTextOnly)
                        return null;

                    Points = int.Parse(words[0]);
                    X = double.Parse(words[1]);
                    Y = double.Parse(words[2]);
                    R = double.Parse(words[3]);

                    ret = new Figure(Points, X, Y, R);
                    break;

                case 5: //Figure(int _points, double _x, double _y, double _r, double _rotation)
                    if (isTextOnly)
                        return null;
                    Points = int.Parse(words[0]);
                    X = double.Parse(words[1]);
                    Y = double.Parse(words[2]);
                    R = double.Parse(words[3]);
                    Rotation = double.Parse(words[4]);

                    ret = new Figure(Points, X, Y, R, Rotation);
                    break;

                case 6: //Figure(string _text, int _x, int _y, int _foregroundRed, int _foregroundBlue, int _foregroundGreen)
                    // Text = words[0]
                    _x = int.Parse(words[1]);
                    _y = int.Parse(words[2]);
                    _r = int.Parse(words[3]);
                    _g = int.Parse(words[4]);
                    _b = int.Parse(words[5]);

                    ret = new TextFigure(words[0], _x, _y, _r, _g, _b);
                    break;

                case 8: //Figure(string _text, int _x, int _y, string _font, int _size, int _foregroundRed, int _foregroundBlue, int _foregroundGreen)
                    // Text = words[0]
                    _x = int.Parse(words[1]);
                    _y = int.Parse(words[2]);
                    // font = words[3]
                    var _size = int.Parse(words[4]);
                    _r = int.Parse(words[5]);
                    _g = int.Parse(words[6]);
                    _b = int.Parse(words[7]);

                    ret = new TextFigure(words[0], _x, _y, words[3], _size, _r, _g, _b);
                    break;
            }

            return ret;
        }
    }
}