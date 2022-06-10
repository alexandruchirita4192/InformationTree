using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class MainFormColorTextChangedHandler : IRequestHandler<MainFormColorTextChangedRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public MainFormColorTextChangedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(MainFormColorTextChangedRequest request, CancellationToken cancellationToken)
        {
            if (request.SelectedTreeNode is not TreeNode selectedNode)
                return null;
            if (request.ColorTextBox is not TextBox tbColor)
                return null;
            
            var color = tbColor.Text
                .ToColor();
            if (color == null)
                return null;
            
            if (request.ChangeType == ColorChangeType.TextColor)
                selectedNode.ForeColor = color.Value;
            else if (request.ChangeType == ColorChangeType.BackColor)
                selectedNode.BackColor = color.Value;

            // on font changed is added too??
            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = false
            };
            return await _mediator.Send(setTreeStateRequest, cancellationToken);
        }
    }
}