using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormFontSizeValueChangedHandler : IRequestHandler<MainFormFontSizeValueChangedRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public MainFormFontSizeValueChangedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(MainFormFontSizeValueChangedRequest request, CancellationToken cancellationToken)
        {
            if (request.SelectedTreeNode is not TreeNode selectedNode)
                return null;
            if (request.FontSizeNumericUpDown is not NumericUpDown nudFontSize)
                return null;

            var oldFont = selectedNode.NodeFont;
            if (oldFont != null)
            {
                selectedNode.NodeFont = new Font(oldFont.FontFamily, (float)nudFontSize.Value, oldFont.Style);

                // on font changed is added too??
                var setTreeStateRequest = new SetTreeStateRequest
                {
                    TreeUnchanged = false
                };
                await _mediator.Send(setTreeStateRequest, cancellationToken);
            }

            return new BaseResponse();
        }
    }
}