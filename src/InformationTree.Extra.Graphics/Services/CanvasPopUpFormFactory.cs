using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Forms;

namespace InformationTree.Extra.Graphics.Services
{
    public class CanvasPopUpFormFactory : ICanvasFormFactory
    {
        private readonly IGraphicsFileFactory _graphicsFileFactory;

        public CanvasPopUpFormFactory(IGraphicsFileFactory graphicsFileFactory)
        {
            _graphicsFileFactory = graphicsFileFactory;
        }

        public ICanvasForm Create(string[] figureLines)
        {
            var graphicsFile = _graphicsFileFactory.CreateGraphicsFile();
            graphicsFile.ParseLines(figureLines);
            var canvasForm = new CanvasPopUpForm(graphicsFile);
            return canvasForm;
        }
    }
}