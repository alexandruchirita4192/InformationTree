using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Domain;
using InformationTree.Extra.Graphics.Services;

namespace InformationTree.Extra.Graphics.Computation
{
    public class GraphicsFileFactory : IGraphicsFileFactory
    {
        #region Constants

        public const double DefaultX = 200d;
        public const double DefaultY = 200d;
        public const int DefaultNumber = 6;
        public const int DefaultPoints = 0;
        public const double DefaultTheta = 0d;
        public const double DefaultTreshold = 0.01d;

        #endregion Constants

        private readonly IGraphicsParser _graphicsParsing;

        public GraphicsFileFactory(IGraphicsParser graphicsParsing)
        {
            _graphicsParsing = graphicsParsing;
        }

        #region Methods

        public IGraphicsFile CreateGraphicsFile()
        {
            var graphicsFile = new GraphicsFile(_graphicsParsing);
            return graphicsFile;
        }

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

        public IGraphicsFile GetDefaultGraphicsFile(int screenBoundsHeight, int screenBoundsWidth)
        {
            var ret = new GraphicsFile(_graphicsParsing);

            var y = screenBoundsHeight / 2;
            var x = screenBoundsWidth / 2;
            var maxScreen = screenBoundsHeight > screenBoundsWidth ? screenBoundsHeight : screenBoundsWidth;
            var max = (maxScreen / 2.0) * Math.Sqrt(2.0d);

            var lines = new List<string>
            {
                "ComputeXComputeY 4 200 200 45 0 0 -1 0"
            };

            for (int i = 50; i <= max; i += 4)
            {
                lines.Add("AddFrame");
                lines.Add($"ComputeXComputeY 4 200 200 {i} 0 0 -1 0");
            }
            lines.Add($"AllCenter 200 200 {x} {y}");
            lines.Add("Cycle 1");

            ret.ParseLines(lines.ToArray());

            return ret;
        }

        public IGraphicsFile GetHardCodedPrettyFigures(int screenBoundsHeight, int screenBoundsWidth)
        {
            var y = screenBoundsHeight / 2;
            var x = screenBoundsWidth / 2;

            var graphicsFile = new GraphicsFile(_graphicsParsing);

            var lines = new List<string>
            {
                "ComputeXComputeY 4 200 200 50 0 0 -1 0",
                "AddFrame",
                "ComputeXComputeY 4 200 200 50 0 6 0 0",
                "AddFrame",
                "ComputeXComputeY 4 200 200 50 0 6 1 0",
                "AddFrame",
                "ComputeXComputeY 4 200 200 50 0 6 2 0",
                "AddFrame",
                "ComputeXComputeY 5 200 200 50 0 6 1 0",
                "AddFrame",
                "ComputeXComputeY 6 200 200 50 0 6 1 0",
                $"AllCenter 200 200 {x} {y}",
                "Cycle 200"
            };

            graphicsFile.ParseLines(lines.ToArray());

            return graphicsFile;
        }

        #endregion Methods

        #region IGraphicsFileRecursiveGenerator

        double IGraphicsFileFactory.DefaultX => DefaultX;
        double IGraphicsFileFactory.DefaultY => DefaultY;
        int IGraphicsFileFactory.DefaultNumber => DefaultNumber;
        int IGraphicsFileFactory.DefaultPoints => DefaultPoints;
        double IGraphicsFileFactory.DefaultTheta => DefaultTheta;
        double IGraphicsFileFactory.DefaultTreshold => DefaultTreshold;

        #endregion IGraphicsFileRecursiveGenerator
    }
}