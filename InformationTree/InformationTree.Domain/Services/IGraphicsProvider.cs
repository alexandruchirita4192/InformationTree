using System.Collections.Generic;

namespace InformationTree.Domain.Services
{
    // TODO: Name this "provider" better; before it was named GraphicsGenerator
    public interface IGraphicsProvider
    {
        List<string> ComputeXComputeY(double radius, int iterations, int computeType = 0);

        List<string> ComputeXComputeY(int points, double x, double y, double radius, double theta, int number, int iterations, int computeType = 0);
    }
}