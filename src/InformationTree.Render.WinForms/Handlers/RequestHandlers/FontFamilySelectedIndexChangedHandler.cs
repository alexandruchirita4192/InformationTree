using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using MediatR;

namespace InformationTree.Render.WinForms.Handlers.RequestHandlers
{
    public class FontFamilySelectedIndexChangedHandler : IRequestHandler<FontFamilySelectedIndexChangedRequest, BaseResponse>
    {
        private readonly IMediator _mediator;

        public FontFamilySelectedIndexChangedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(FontFamilySelectedIndexChangedRequest request, CancellationToken cancellationToken)
        {
            if (request.TreeView is not TreeView tvTaskList)
                return null;
            if (request.FontFamilyComboBox is not ComboBox cbFontFamily)
                return null;
            if (request.FontSizeNumericUpDown is not NumericUpDown nudFontSize)
                return null;

            if (tvTaskList.SelectedNode != null && cbFontFamily.SelectedItem != null)
            {
                var oldFont = tvTaskList.SelectedNode.NodeFont;

                if (cbFontFamily.SelectedItem is string fontFamily)
                    tvTaskList.SelectedNode.NodeFont = new Font(fontFamily, (float)nudFontSize.Value, oldFont.Style);

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