using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationTree.Graphics
{
    public static class Graphics
    {
        public static readonly int FramesPerSecond = 60;
        public const double GraphicsPI = 3.14159265358979323846;

        public static double ComputeX(double radius, double theta, int step, int points)
        {
            var pointsDouble = (double) points;
            var stepDouble = (double) step;

            return ((radius) * Math.Cos(((theta * GraphicsPI) / 90.0) + ((2.0 * (stepDouble) * GraphicsPI) / (pointsDouble))));
        }

        public static double ComputeY(double radius, double theta, int step, int points)
        {
            var pointsDouble = (double) points;
            var stepDouble = (double) step;

            return ((radius) * Math.Sin(((theta * GraphicsPI) / 90.0) + ((2.0 * (stepDouble) * GraphicsPI) / (pointsDouble))));
        }
    }
}
