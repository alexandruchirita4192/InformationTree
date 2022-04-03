using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Services.FileParsing;
using InformationTree.Forms;

namespace InformationTree.Extra.Graphics.Services
{
    public class CanvasPopUpFormFactory : ICanvasFormFactory
    {
        private readonly IGraphicsFileRecursiveGenerator _graphicsProvider;

        public CanvasPopUpFormFactory(IGraphicsFileRecursiveGenerator graphicsProvider)
        {
            _graphicsProvider = graphicsProvider;
        }

        public ICanvasForm Create(string[] figureLines)
        {
            var graphicsFile = new GraphicsFile(_graphicsProvider);
            graphicsFile.ParseLines(figureLines);
            var canvasForm = new CanvasPopUpForm(graphicsFile);
            return canvasForm;
        }
    }
}