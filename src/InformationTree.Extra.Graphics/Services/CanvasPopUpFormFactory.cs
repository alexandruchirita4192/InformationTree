using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Forms;
using MediatR;

namespace InformationTree.Extra.Graphics.Services
{
    public class CanvasPopUpFormFactory : ICanvasFormFactory
    {
        private readonly IGraphicsFileFactory _graphicsFileFactory;
        private readonly IMediator _mediator;

        public CanvasPopUpFormFactory(
            IGraphicsFileFactory graphicsFileFactory,
            IMediator mediator)
        {
            _graphicsFileFactory = graphicsFileFactory;
            _mediator = mediator;
        }

        public ICanvasForm Create(string[] figureLines)
        {
            var graphicsFile = _graphicsFileFactory.CreateGraphicsFile();
            graphicsFile.ParseLines(figureLines);
            var canvasForm = new CanvasPopUpForm(_mediator, graphicsFile);
            return canvasForm;
        }
    }
}