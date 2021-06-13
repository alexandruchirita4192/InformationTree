namespace FormsGame.Graphics
{
    public class Rotation
    {
        #region Properties

        public double X { get; set; }
        public double Y { get; set; }
        public double R { get; set; }
        public double RotationValue { get; set; }
        public double RotationIncrement { get; set; }

        public Rotation NextRotation { get; set; }

        #endregion Properties

        #region Constructors

        public Rotation() : this(0.0, 0.0, 0.0, 0.0, 0.0, null)
        {
        }

        public Rotation(double x, double y, double r, double rotationValue, Rotation nextRotation) : this(x, y, r, rotationValue, 0.0, nextRotation)
        {
        }

        public Rotation(double x, double y, double r, double rotationValue, double rotationIncrement, Rotation nextRotation)
        {
            X = x; Y = y; R = r; RotationValue = rotationValue; RotationIncrement = rotationIncrement; NextRotation = nextRotation;
        }

        #endregion Constructors

        #region Methods

        public string GetDebugText()
        {
            var debugText = string.Empty;
            if (X != 0)
                debugText += "x=" + X.ToString() + ";";
            if (Y != 0)
                debugText += "y=" + Y.ToString() + ";";
            if (R != 0)
                debugText += "r=" + R.ToString() + ";";
            if (RotationValue != 0)
                debugText += "rotation=" + RotationValue.ToString() + ";";
            if (RotationIncrement != 0)
                debugText += "addRotation=" + RotationIncrement.ToString() + ";";
            return debugText;
        }

        #endregion Methods
    };
}