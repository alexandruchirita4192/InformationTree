using System.Text;
using InformationTree.Extra.Graphics.Computation;
using InformationTree.Extra.Graphics.Extensions;
using D = System.Drawing;

namespace InformationTree.Extra.Graphics.Domain
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

        public Figure(int points, double x, double y, double radius) : this()
        {
            Points = points;
            X = x;
            Y = y;
            Radius = radius;
        }

        public Figure(int points, double x, double y, double radius, double rotation) : this()
        {
            Points = points;
            X = x;
            Y = y;
            Radius = radius;
            Rotation = rotation;
        }

        #endregion Constructors

        #region Methods

        public override void Show(D.Graphics graphics)
        {
            if (Points < 0)
                return;

            if (Points == 0) // circle
            {
                var color = Color.FromArgb((int)(Red * 255.0), (int)(Green * 255.0), (int)(Blue * 255.0));
                var pen = new Pen(color);
                graphics.DrawCircle(pen, (float)X, (float)Y, (float)Radius);
            }
            else if (Points == 1) // point
            {
                var color = Color.FromArgb((int)(Red * 255.0), (int)(Green * 255.0), (int)(Blue * 255.0));
                var pen = new Pen(color);
                var point = new Point((int)X, (int)Y);
                graphics.DrawLine(pen, point, point);
            }
            else // other
            {
                var color = Color.FromArgb((int)(Red * 255.0), (int)(Green * 255.0), (int)(Blue * 255.0));
                var pen = new Pen(color);
                var pointList = new List<PointF>();

                for (int i = 0; i < Points; i++)
                {
                    var xx = X + GraphicsComputation.ComputeX(Radius, Rotation, i, Points);
                    var yy = Y + GraphicsComputation.ComputeY(Radius, Rotation, i, Points);

                    pointList.Add(new D.PointF((float)xx, (float)yy));
                }

                pointList.Add(pointList[0]);

                graphics.DrawLines(pen, pointList.ToArray());
            }
        }

        public void Move(double x, double y, double radius)
        {
            if (Points == 0 || Points > 1)
            {
                X = x;
                Y = y;
                Radius = radius;
            }
            else
                throw new Exception("Not enough data to move a figure with " + Points.ToString() + " points!");
        }

        public override string GetDebugText()
        {
            var stringBuilder = new StringBuilder();
            if (X != 0)
                stringBuilder.Append($"x={X};");
            if (Y != 0)
                stringBuilder.Append($"y={Y};");
            if (Radius != 0)
                stringBuilder.Append($"r={Radius};");
            if (Rotation != 0)
                stringBuilder.Append($"rotation={Rotation};");
            if (AddRotation != 0)
                stringBuilder.Append($"addRotation={AddRotation};");

            Rotation? c = Rot;
            if (c != null)
                stringBuilder.AppendLine();

            while (c != null)
            {
                stringBuilder.AppendLine($"Rotation({c.GetDebugText()});");
                c = c.NextRotation;
            }

            return stringBuilder.ToString();
        }

        #endregion Methods
    };
}