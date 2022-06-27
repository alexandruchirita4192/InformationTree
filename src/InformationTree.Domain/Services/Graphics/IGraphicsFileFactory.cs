using System.Collections.Generic;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Extra.Graphics.Domain;

namespace InformationTree.Domain.Services.Graphics
{
    public interface IGraphicsFileFactory
    {
        double DefaultX { get; }
        double DefaultY { get; }
        int DefaultNumber { get; }
        int DefaultPoints { get; }
        double DefaultTheta { get; }
        double DefaultTreshold { get; }

        IGraphicsFile CreateGraphicsFile();

        List<string> GenerateFigureLines(double radius, int iterations, ComputeType computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners);

        List<string> GenerateFigureLines(int points, double x, double y, double radius, double theta, int number, int iterations, ComputeType computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners);

        IGraphicsFile GetDefaultGraphicsFile(int screenBoundsHeight, int screenBoundsWidth);

        IGraphicsFile GetHardCodedPrettyFigures(int screenBoundsHeight, int screenBoundsWidth);
    }
}