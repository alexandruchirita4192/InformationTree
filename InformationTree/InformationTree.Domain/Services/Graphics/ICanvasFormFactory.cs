using InformationTree.Domain.Entities.Graphics;

namespace InformationTree.Domain.Services.Graphics
{
    public interface ICanvasFormFactory
    {
        ICanvasForm Create(string[] figureLines);
    }
}