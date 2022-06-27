using InformationTree.Domain.Entities.Graphics;

namespace InformationTree.Domain.Services.Graphics
{
    public interface IGraphicsParser
    {
        void ParseLines(IFrame frame, System.Timers.Timer runTimer, string[] lines);
    }
}