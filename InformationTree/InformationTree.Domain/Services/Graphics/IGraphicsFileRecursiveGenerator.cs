using System.Collections.Generic;
using InformationTree.Extra.Graphics.Domain;

namespace InformationTree.Domain.Services.Graphics
{
    // TODO: Name this "provider" better; before it was named GraphicsGenerator
    public interface IGraphicsFileRecursiveGenerator
    {
        List<string> GenerateGraphicsFileLines(double radius, int iterations, ComputeType computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners);

        List<string> GenerateFigureLines(int points, double x, double y, double radius, double theta, int number, int iterations, ComputeType computeType = ComputeType.ExtraFiguresWithPointsNumberOfCorners);
    }
}