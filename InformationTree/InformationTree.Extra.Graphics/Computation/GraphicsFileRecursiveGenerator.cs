using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Domain;

namespace InformationTree.Extra.Graphics.Computation
{
    public class GraphicsFileRecursiveGenerator : IGraphicsFileRecursiveGenerator
    {
        #region Properties

        public const double DefaultX = 200d;
        public const double DefaultY = 200d;
        public const int DefaultNumber = 6;
        public const int DefaultPoints = 0;
        public const double DefaultTheta = 0d;
        public const double DefaultTreshold = 0.01d;

        #endregion Properties

        #region Methods

        public List<string> GenerateFigureLines(int points, double x, double y, double radius, double theta, int number, int iterations, ComputeType computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners)
        {
            var lines = new List<string>();

            if (iterations < -1)
                return lines;

            if (computeType == ComputeType.Generic)
            {
                // The generic calculation does not add anything extra
            }
            else if (computeType == ComputeType.ExtraFiguresWithPointsNumberOfCorners)
                lines.Add($"{GraphicsFileConstants.AddFigureOnce.DefaultName} {FigureType.Figure} {points} {x} {y} {radius}");
            else if (computeType == ComputeType.ExtraCircles)
                lines.Add($"{GraphicsFileConstants.AddFigureOnce.DefaultName} {FigureType.Figure} 0 {x} {y} {radius}");

            for (int i = 0; i < number; i++)
            {
                var newXrelativeToCircleOnly = Math.Abs(GraphicsComputation.ComputeX(radius, theta, i, number));
                var newYrelativeToCircleOnly = Math.Abs(GraphicsComputation.ComputeY(radius, theta, i, number));

                // Computed X and Y are using the new calculation only if it has a value greater than DefaultTreshold
                var computedX = newXrelativeToCircleOnly > DefaultTreshold ? x + newXrelativeToCircleOnly : x;
                var computedY = newYrelativeToCircleOnly > DefaultTreshold ? y + newYrelativeToCircleOnly : y;

                if (iterations != -1)
                    lines.Add($"{GraphicsFileConstants.AddFigureOnce.DefaultName} {FigureType.Figure} {points} {computedX} {computedY} {radius}");

                // Recursive call for the next iteration
                lines.AddRange(GenerateFigureLines(points, computedX, computedY, radius, theta, number, iterations - 1));
            }

            return lines;
        }

        public List<string> GenerateFigureLines(double radius, int iterations, ComputeType computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners)
        {
            return GenerateFigureLines(DefaultPoints, DefaultX, DefaultY, radius, DefaultTheta, DefaultNumber, iterations, computeType);
        }

        #endregion Methods

        #region IGraphicsFileRecursiveGenerator

        double IGraphicsFileRecursiveGenerator.DefaultX => DefaultX;
        double IGraphicsFileRecursiveGenerator.DefaultY => DefaultY;
        int IGraphicsFileRecursiveGenerator.DefaultNumber => DefaultNumber;
        int IGraphicsFileRecursiveGenerator.DefaultPoints => DefaultPoints;
        double IGraphicsFileRecursiveGenerator.DefaultTheta => DefaultTheta;
        double IGraphicsFileRecursiveGenerator.DefaultTreshold => DefaultTreshold;

        #endregion IGraphicsFileRecursiveGenerator
    }
}