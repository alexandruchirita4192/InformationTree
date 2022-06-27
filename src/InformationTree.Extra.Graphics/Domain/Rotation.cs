using System.Text;

namespace InformationTree.Extra.Graphics.Domain
{
    public class Rotation
    {
        #region Properties

        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
        public double RotationValue { get; set; }
        public double RotationIncrement { get; set; }

        public Rotation? NextRotation { get; set; }

        #endregion Properties

        #region Constructors

        public Rotation() : this(0.0, 0.0, 0.0, 0.0, 0.0, null)
        {
        }

        public Rotation(double x, double y, double radius, double rotationValue, Rotation? nextRotation) : this(x, y, radius, rotationValue, 0.0, nextRotation)
        {
        }

        public Rotation(double x, double y, double radius, double rotationValue, double rotationIncrement, Rotation? nextRotation)
        {
            X = x; Y = y; Radius = radius; RotationValue = rotationValue; RotationIncrement = rotationIncrement; NextRotation = nextRotation;
        }

        #endregion Constructors

        #region Methods

        public string GetDebugText()
        {
            var debugTextSb = new StringBuilder();
            if (X != 0)
                debugTextSb.Append($"x={X};");
            if (Y != 0)
                debugTextSb.Append($"y={Y};");
            if (Radius != 0)
                debugTextSb.Append($"radius={Radius};");
            if (RotationValue != 0)
                debugTextSb.Append($"rotation={RotationValue};");
            if (RotationIncrement != 0)
                debugTextSb.Append($"addRotation={RotationIncrement};");
            return debugTextSb.ToString();
        }

        #endregion Methods
    };
}