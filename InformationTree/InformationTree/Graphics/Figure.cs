using System;
using System.Collections.Generic;
using System.Text;
using D = System.Drawing;

namespace InformationTree.Graphics
{
    public class Figure : BaseFigure
    {
        #region Properties

        public override FigureType FigureType
        {
            get
            {
                if (Points == 0)
                    return FigureType.Circle;
                else if (Points == 1)
                    return FigureType.Point;
                else if (Points == 2)
                    return FigureType.Line;
                else if (Points > 0)
                    return FigureType.Polygon;
                else
                    return FigureType.None;
            }
        }

        public double Radius { get; protected set; }

        #endregion Properties

        #region Constructors

        public Figure()
        {
            Points = -1;
            X = Y = Radius = -1;
            Rotation = 0.0;
            AddRotation = 0.0;
            Rot = null;

            SetColor();
        }

        public Figure(int _points, double _x, double _y, double _r) : this()
        {
            Points = _points;
            X = _x;
            Y = _y;
            Radius = _r;
        }

        public Figure(int _points, double _x, double _y, double _r, double _rotation) : this()
        {
            Points = _points;
            X = _x;
            Y = _y;
            Radius = _r;
            Rotation = _rotation;
        }

        #endregion Constructors

        #region Methods

        public override void Show(D.Graphics graphics)
        {
            if (Points < 0)
                return;

            if (Points == 0) // circle
            {
                var color = D.Color.FromArgb((int)(Red * 255.0), (int)(Green * 255.0), (int)(Blue * 255.0));
                var pen = new D.Pen(color);
                graphics.DrawCircle(pen, (float)X, (float)Y, (float)Radius);
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
                var color = D.Color.FromArgb((int)(Red * 255.0), (int)(Green * 255.0), (int)(Blue * 255.0));
                var pen = new D.Pen(color);
                var pointList = new List<D.PointF>();

                for (int i = 0; i < Points; i++)
                {
                    var xx = X + Graphics.ComputeX(Radius, Rotation, i, Points);
                    var yy = Y + Graphics.ComputeY(Radius, Rotation, i, Points);

                    pointList.Add(new D.PointF((float)xx, (float)yy));
                }

                pointList.Add(pointList[0]);

                graphics.DrawLines(pen, pointList.ToArray());
            }
        }


        public void Move(double _x, double _y, double _r)
        {
            if (Points == 0 || Points > 1)
            {
                X = _x;
                Y = _y;
                Radius = _r;
            }
            else
                throw new Exception("Not enough data to move a figure with " + Points.ToString() + " points!");
        }

        public override string GetDebugText()
        {
            var stringBuilder = new StringBuilder();
            if (X != 0)
                stringBuilder.Append("x=" + X.ToString() + ";");
            if (Y != 0)
                stringBuilder.Append("y=" + Y.ToString() + ";");
            if (Radius != 0)
                stringBuilder.Append("r=" + Radius.ToString() + ";");
            if (Rotation != 0)
                stringBuilder.Append("rotation=" + Rotation.ToString() + ";");
            if (AddRotation != 0)
                stringBuilder.Append("addRotation=" + AddRotation.ToString() + ";");

            Rotation c = Rot;
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
