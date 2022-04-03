using System.Collections.Generic;
using InformationTree.Extra.Graphics.Domain;

namespace InformationTree.Domain.Services.Graphics
{
    public interface IGraphicsFileRecursiveGenerator
    {
        double DefaultX { get; }
        double DefaultY { get; }
        int DefaultNumber { get; }
        int DefaultPoints { get; }
        double DefaultTheta { get; }
        double DefaultTreshold { get; }

        List<string> GenerateFigureLines(double radius, int iterations, ComputeType computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners);

        List<string> GenerateFigureLines(int points, double x, double y, double radius, double theta, int number, int iterations, ComputeType computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners);
    }
}