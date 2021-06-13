using System;
using System.Collections.Generic;
using System.Text;
using D = System.Drawing;

namespace FormsGame.Graphics
{
    public enum FigureType { None, Circle, Point, Line, Polygon, Text, Arc };

    public class Figure // TODO: Clean this whole class!!!
    {
        #region Properties

        #region Private

        private double Red, Blue, Green;
        private double rotation, addRotation;
        private Rotation rot;
        private bool isText;
        private Txt t;
        private double thetaFrom, thetaTo;

        #endregion Private

        #region Public

        public double X { get; private set; }
        public double Y { get; private set; }
        public double R { get; private set; }
        public int Points { get; set; }
        public bool IsArc { get; set; } // unused though

        public FigureType FigureType
        {
            get
            {
                if (Points == 0)
                    return FigureType.Circle;
                else if (Points == 1)
                    return FigureType.Point;
                else if (Points == 2)
                    return FigureType.Line;
                else if ((Points > 0) && (!IsArc) && (!isText))
                    return FigureType.Polygon;
                else if (isText)
                    return FigureType.Text;
                else if (IsArc)
                    return FigureType.Arc;
                else
                    return FigureType.None;
            }
        }

        #region Text
        public string Text
        {
            get
            {
                if (isText && t != null)
                    return t.Text;
                return null;
            }
            set
            {
                if (isText && t != null)
                    t.Text = value;
            }
        }
        #endregion Text

        #region ThetaFrom
        public double? ThetaFrom
        {
            get
            {
                if (IsArc)
                    return thetaFrom;
                return null;
            }
            set
            {
                if (IsArc && value.HasValue)
                    thetaFrom = value.Value;
            }
        }
        #endregion ThetaFrom

        #region ThetaTo
        public double? ThetaTo
        {
            get
            {
                if (IsArc)
                    return thetaTo;
                return null;
            }
            set
            {
                if (IsArc && value.HasValue)
                    thetaTo = value.Value;
            }
        }
        #endregion ThetaTwo

        #endregion Public

        #endregion Properties

        #region Constructors

        public Figure()
        {
            Points = -1;
            Red = 0;
            Green = 0;
            Blue = 0;
            X = Y = R = -1;
            rotation = 0.0;
            addRotation = 0.0;
            rot = null;
            isText = false;
            t = null;
            IsArc = false;
            thetaFrom = 0.0;
            thetaTo = 0.0;
            SetColor();
        }

        public Figure(string _text, int _x, int _y) : this()
        {
            t = new Txt(_text, _x, _y);
            isText = true;
        }

        public Figure(string _text, int _x, int _y, int _foregroundRed, int _foregroundBlue, int _foregroundGreen) : this()
        {
            t = new Txt(_text, _x, _y, _foregroundRed, _foregroundBlue, _foregroundGreen);
            isText = true;
        }

        public Figure(string _text, int _x, int _y, string _font, int _size, int _foregroundRed, int _foregroundBlue, int _foregroundGreen) : this()
        {
            t = new Txt(_text, _x, _y, _font, _size, _foregroundRed, _foregroundBlue, _foregroundGreen);
            isText = true;
        }

        public Figure(int _points, double _x, double _y, double _r) : this()
        {
            Points = _points;
            X = _x;
            Y = _y;
            R = _r;
        }

        public Figure(int _points, double _x, double _y, double _r, double _rotation) : this()
        {
            Points = _points;
            X = _x;
            Y = _y;
            R = _r;
            rotation = _rotation;
        }

        public Figure(string _line, bool isTextOnly = false) : this()
        {
            int _x, _y, _r, _g, _b, _level;

            var words = _line.Split(' ');
            switch (words.Length)
            {
                case 2: //Figure(string _text, int _level)
                    isText = true;
                    // Text = words[0]
                    _level = int.Parse(words[1]);
                    t = new Txt(words[0], _level);
                    break;
                case 3: //Figure(string _text, int _x, int _y)
                    isText = true;
                    // Text = words[0]
                    _x = int.Parse(words[1]);
                    _y = int.Parse(words[2]);
                    t = new Txt(words[0], _x, _y);
                    break;
                case 4: //Figure(int _points, double _x, double _y, double _r)
                    if (isTextOnly)
                        return;
                    Points = int.Parse(words[0]);
                    X = double.Parse(words[1]);
                    Y = double.Parse(words[2]);
                    R = double.Parse(words[3]);
                    break;
                case 5: //Figure(int _points, double _x, double _y, double _r, double _rotation)
                    if (isTextOnly)
                        return;
                    Points = int.Parse(words[0]);
                    X = double.Parse(words[1]);
                    Y = double.Parse(words[2]);
                    R = double.Parse(words[3]);
                    rotation = double.Parse(words[4]);
                    break;
                case 6: //Figure(string _text, int _x, int _y, int _foregroundRed, int _foregroundBlue, int _foregroundGreen)
                    isText = true;
                    // Text = words[0]
                    _x = int.Parse(words[1]);
                    _y = int.Parse(words[2]);
                    _r = int.Parse(words[3]);
                    _g = int.Parse(words[4]);
                    _b = int.Parse(words[5]);
                    t = new Txt(words[0], _x, _y, _r, _g, _b);
                    break;
                case 8: //Figure(string _text, int _x, int _y, string _font, int _size, int _foregroundRed, int _foregroundBlue, int _foregroundGreen)
                    isText = true;
                    // Text = words[0]
                    _x = int.Parse(words[1]);
                    _y = int.Parse(words[2]);
                    // font = words[3]
                    var _size = int.Parse(words[4]);
                    _r = int.Parse(words[5]);
                    _g = int.Parse(words[6]);
                    _b = int.Parse(words[7]);
                    t = new Txt(words[0], _x, _y, words[3], _size, _r, _g, _b);
                    break;
            }
        }

        #endregion Constructors

        #region Methods
        public void SetText(string t, int x, int y)
        {
            if (this.t == null)
            {
                this.t = new Txt(t, x, y);
                isText = true;
            }
        }

        public void Rotate()
        {
            rotation += addRotation;
        }

        public void InitRotate(double _rotation)
        {
            rotation = 0.0;
            addRotation = _rotation;
        }

        public void RotateAround()
        {
            if (rot == null)
                return;

            Rotation c = rot;

            while (c != null)
            {
                c.RotationValue += c.RotationIncrement;
                if (c.NextRotation != null)
                {
                    c.NextRotation.X = c.X + Graphics.ComputeX(c.R, c.RotationValue, 0, 1);
                    c.NextRotation.Y = c.Y + Graphics.ComputeY(c.R, c.RotationValue, 0, 1);
                }
                else
                {
                    X = c.X + Graphics.ComputeX(c.R, c.RotationValue, 0, 1);
                    Y = c.Y + Graphics.ComputeY(c.R, c.RotationValue, 0, 1);
                }

                c = c.NextRotation;
            }
        }

        public void AddRotateAround(double _r, double _addRotation, double _angle)
        {
            if (rot == null)
            {
                rot = new Rotation(X, Y, _r, _angle, _addRotation, null);
            }
            else
            {
                Rotation c = rot;
                while (c.NextRotation != null)
                    c = c.NextRotation;

                c.NextRotation = new Rotation(c.X, c.Y, _r, _angle, _addRotation, null);
            }
        }

        public void AddRotateAround(double _r, double _addRotation)
        {
            AddRotateAround(_r, _addRotation, 0);
        }

        public void Show(D.Graphics graphics)
        {
            if (isText)
            {
                if (t != null)
                    t.Show(graphics);
                return;
            }

            if (Points < 0)
                return;

            if (Points == 0) // circle
            {
                var color = D.Color.FromArgb((int)(Red * 255.0), (int)(Green * 255.0), (int)(Blue * 255.0));
                var pen = new D.Pen(color);
                graphics.DrawCircle(pen, (float)X, (float)Y, (float)R);
            }
            else if (Points == 1) // point
            {
                var color = D.Color.FromArgb((int)(Red * 255.0), (int)(Green * 255.0), (int)(Blue * 255.0));
                var pen = new D.Pen(color);
                var point = new D.Point((int)X, (int)Y);
                graphics.DrawLine(pen, point, point);
            }
            else // other
            {
                if (Points < 0)
                    return;

                var color = D.Color.FromArgb((int)(Red * 255.0), (int)(Green * 255.0), (int)(Blue * 255.0));
                var pen = new D.Pen(color);
                var pointList = new List<D.PointF>();

                for (int i = 0; i < Points; i++)
                {
                    var xx = X + Graphics.ComputeX(R, rotation, i, Points);
                    var yy = Y + Graphics.ComputeY(R, rotation, i, Points);

                    pointList.Add(new D.PointF((float)xx, (float)yy));
                }

                pointList.Add(pointList[0]);

                graphics.DrawLines(pen, pointList.ToArray());
            }
        }

        public void Move(double _x, double _y)
        {
            if (isText)
            {
                if (t != null)
                    t.Move(_x, _y);
            }
            else
            {
                X = _x;
                Y = _y;

                //if (points == 1)
                //{
                //}
                //else
                //    throw new Exception("Not enough data to move a figure with " + points.ToString() + " points!");
            }
        }


        public void Move(double _x, double _y, double _r)
        {
            if (isText)
                throw new Exception("Text does not require _r parameter!");

            if (Points == 0 || Points > 1)
            {
                X = _x;
                Y = _y;
                R = _r;
            }
            else
                throw new Exception("Not enough data to move a figure with " + Points.ToString() + " points!");
        }

        public void ChangeTheta(double thetaFrom, double thetaTo)
        {
            if (!IsArc)
                return;
            ThetaFrom = thetaFrom;
            ThetaTo = thetaTo;
        }

        public void SetColor(double r = 1.0, double g = 1.0, double b = 1.0)
        {
            if (isText && t != null)
            {
                int _r = (int)(r * 255.0);
                int _g = (int)(g * 255.0);
                int _b = (int)(b * 255.0);
                t.SetColor(_r, _g, _b);
            }
            else
            {
                Red = r;
                Green = g;
                Blue = b;
            }
        }

        public void SetFont(string _font, double _size)
        {
            if (isText && t != null)
                t.SetFont(_font, _size);
        }

        public void SetText(string _text)
        {
            if (isText && t != null)
                t.SetText(_text);
        }

        public string GetDebugText()
        {
            var stringBuilder = new StringBuilder();
            if (X != 0)
                stringBuilder.Append("x=" + X.ToString() + ";");
            if (Y != 0)
                stringBuilder.Append("y=" + Y.ToString() + ";");
            if (R != 0)
                stringBuilder.Append("r=" + R.ToString() + ";");
            if (rotation != 0)
                stringBuilder.Append("rotation=" + rotation.ToString() + ";");
            if (addRotation != 0)
                stringBuilder.Append("addRotation=" + addRotation.ToString() + ";");
            if (!string.IsNullOrEmpty(Text))
                stringBuilder.Append("Text=" + Text.ToString() + ";");

            Rotation c = rot;
            if (c != null)
                stringBuilder.AppendLine();

            while (c != null)
            {
                stringBuilder.AppendLine("Rotation(" + c.GetDebugText() + ");");
                c = c.NextRotation;
            }

            return stringBuilder.ToString();
        }

        #endregion Methods
    };
}
