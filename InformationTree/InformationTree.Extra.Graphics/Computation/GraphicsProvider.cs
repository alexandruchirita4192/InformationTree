using System;
using System.Collections.Generic;
using InformationTree.Domain.Services;

namespace InformationTree.Extra.Graphics.Computation
{
    public class GraphicsProvider : IGraphicsProvider
    {
        #region Properties

        public const double DefaultX = 200;
        public const double DefaultY = 200;
        public const int DefaultNumber = 6;
        public const int DefaultPoints = 0;
        public const double DefaultTheta = 0;

        #endregion Properties

        #region Methods

        public List<string> ComputeXComputeY(int points, double x, double y, double radius, double theta, int number, int iterations, int computeType = 0)
        {
            List<string> lines = new List<string>();

            if (iterations >= -1)
            {
                int i;
                if (computeType == 0)
                    lines.Add("AddFigureOnce Figure " + points + " " + x + " " + y + " " + radius);
                else if (computeType == 1)
                    lines.Add("AddFigureOnce Figure 0 " + x + " " + y + " " + radius);

                if (iterations != -1)
                    for (i = 0; i < number; i++)
                    {
                        double cpx0 = Math.Abs(GraphicsComputation.ComputeX(radius, theta, i, number)) > 0.01 ? x + GraphicsComputation.ComputeX(radius, theta, i, number) : x;
                        double cpy0 = Math.Abs(GraphicsComputation.ComputeY(radius, theta, i, number)) > 0.01 ? y + GraphicsComputation.ComputeY(radius, theta, i, number) : y;

                        lines.Add("AddFigureOnce Figure " + points + " " + cpx0 + " " + cpy0 + " " + radius);
                    }

                for (i = 0; i < number; i++)
                {
                    double cpx0 = Math.Abs(GraphicsComputation.ComputeX(radius, theta, i, number)) > 0.01 ? x + GraphicsComputation.ComputeX(radius, theta, i, number) : x;
                    double cpy0 = Math.Abs(GraphicsComputation.ComputeY(radius, theta, i, number)) > 0.01 ? y + GraphicsComputation.ComputeY(radius, theta, i, number) : y;

                    lines.AddRange(ComputeXComputeY(points, cpx0, cpy0, radius, theta, number, iterations - 1));
                }
            }

            return lines;
        }

        public List<string> ComputeXComputeY(double radius, int iterations, int computeType = 0)
        {
            return ComputeXComputeY(DefaultPoints, DefaultX, DefaultY, radius, DefaultTheta, DefaultNumber, iterations, computeType);
        }

        #endregion Methods
    }
}