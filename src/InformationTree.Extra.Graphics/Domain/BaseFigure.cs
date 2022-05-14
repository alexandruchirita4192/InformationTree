using D = System.Drawing;
using InformationTree.Extra.Graphics.Computation;

namespace InformationTree.Extra.Graphics.Domain
{
    public abstract class BaseFigure
    {
        #region Properties

        public abstract FigureType FigureType { get; }
        public int Points { get; set; }

        #region Location

        public double X { get; protected set; }
        public double Y { get; protected set; }

        #endregion Location

        #region Colors

        public double Red { get; protected set; }
        public double Blue { get; protected set; }
        public double Green { get; protected set; }

        #endregion Colors

        #region Rotations

        public double Rotation { get; protected set; }
        public double AddRotation { get; protected set; }
        public Rotation? Rot { get; protected set; }

        #endregion Rotations

        #endregion Properties

        #region Constructor

        public BaseFigure()
        {
            Red = 1;
            Blue = 1;
            Green = 1;
        }

        #endregion Constructor

        #region Methods

        public void Rotate()
        {
            Rotation += AddRotation;
        }

        public void InitRotate(double rotation)
        {
            Rotation = 0.0;
            AddRotation = rotation;
        }

        public void RotateAround()
        {
            if (Rot == null)
                return;

            Rotation? c = Rot;

            while (c != null)
            {
                c.RotationValue += c.RotationIncrement;
                if (c.NextRotation != null)
                {
                    c.NextRotation.X = c.X + GraphicsComputation.ComputeX(c.R, c.RotationValue, 0, 1);
                    c.NextRotation.Y = c.Y + GraphicsComputation.ComputeY(c.R, c.RotationValue, 0, 1);
                }
                else
                {
                    X = c.X + GraphicsComputation.ComputeX(c.R, c.RotationValue, 0, 1);
                    Y = c.Y + GraphicsComputation.ComputeY(c.R, c.RotationValue, 0, 1);
                }

                c = c.NextRotation;
            }
        }

        public void AddRotateAround(double r, double addRotation, double angle)
        {
            if (Rot == null)
            {
                Rot = new Rotation(X, Y, r, angle, addRotation, null);
            }
            else
            {
                Rotation c = Rot;
                while (c.NextRotation != null)
                    c = c.NextRotation;

                c.NextRotation = new Rotation(c.X, c.Y, r, angle, addRotation, null);
            }
        }

        public void AddRotateAround(double r, double addRotation)
        {
            AddRotateAround(r, addRotation, 0);
        }

        public void Move(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void SetColor(double r = 1.0, double g = 1.0, double b = 1.0)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        public abstract void Show(D.Graphics graphics);

        public abstract string GetDebugText();

        #endregion Methods
    }
}