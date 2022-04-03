namespace InformationTree.Extra.Graphics.Computation
{
    /// <summary>
    /// Calculates position of points on a circle.
    /// </summary>
    public static class GraphicsComputation
    {
        public static readonly int FramesPerSecond = 60;
        public static readonly int MillisecondsPerFrame;

        static GraphicsComputation()
        {
            // Calculate milliseconds per frame.
            var secondInMilliseconds = 1000d;
            var framePerSecondDouble = (double)FramesPerSecond;
            var millisecondsPerFrame = secondInMilliseconds / framePerSecondDouble;
            MillisecondsPerFrame = (int)millisecondsPerFrame;
        }

        public const double GraphicsPI = 3.14159265358979323846;

        /// <summary>
        /// Calculates the X position of a point on a circle.
        /// </summary>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="theta">Theta angle rotating the circle.</param>
        /// <param name="step">Current point number being calculated on a circle.</param>
        /// <param name="points">Number of points on the circle.</param>
        /// <returns>X position of a point on a circle.</returns>
        public static double ComputeX(double radius, double theta, int step, int points)
        {
            // Convert variables to double to ensure a double precision calculation even if integers are used as parameters.
            var pointsDouble = (double)points;
            var stepDouble = (double)step;

            return radius * Math.Cos(theta * GraphicsPI / 90.0 + 2.0 * stepDouble * GraphicsPI / pointsDouble);
        }

        /// <summary>
        /// Calculates the Y position of a point on a circle.
        /// </summary>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="theta">Theta angle rotating the circle.</param>
        /// <param name="step">Current point number being calculated on a circle.</param>
        /// <param name="points">Number of points on the circle.</param>
        /// <returns>Y position of a point on a circle.</returns>
        public static double ComputeY(double radius, double theta, int step, int points)
        {
            // Convert variables to double to ensure a double precision calculation even if integers are used as parameters.
            var pointsDouble = (double)points;
            var stepDouble = (double)step;

            return radius * Math.Sin(theta * GraphicsPI / 90.0 + 2.0 * stepDouble * GraphicsPI / pointsDouble);
        }
    }
}